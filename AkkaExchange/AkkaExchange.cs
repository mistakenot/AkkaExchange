using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Akka.Persistence.Query;
using Akka.Persistence.Query.Sql;
using Akka.Streams;
using Akka.Streams.Dsl;
using AkkaExchange.Client;
using AkkaExchange.Client.Actors;
using AkkaExchange.Client.Commands;
using AkkaExchange.Execution.Actors;
using AkkaExchange.Orders.Actors;
using AkkaExchange.Orders.Commands;
using AkkaExchange.Shared.Actors;
using AkkaExchange.Shared.Events;
using AkkaExchange.Shared.Queries;
using AkkaExchange.Utils;
using Autofac;
using Microsoft.Extensions.Logging;

namespace AkkaExchange
{
    public class AkkaExchange : IDisposable
    {
        public IAkkaExchangeQueries Queries { get; }

        private readonly ActorSystem _system;
        private readonly ILogger<AkkaExchange> _logger;
        private readonly IActorRef _clientManager;
        private readonly IActorRef _orderBook;
        private readonly IActorRef _orderExecutorManager;
        private readonly IDisposable _errorStreamLoggerSubscription;

        public AkkaExchange(
            ContainerBuilder container, 
            Config config,
            ILogger<AkkaExchange> logger)
        {
            _system = ActorSystem.Create("akka-exchange-system", config);
            _logger = logger;
            
            var globalActorRefs = new GlobalActorRefs();

            container.RegisterInstance<IGlobalActorRefs>(globalActorRefs);

            var materializer = ActorMaterializer.Create(_system);
            var readJournal = PersistenceQuery.Get(_system).ReadJournalFor<SqlReadJournal>("akka.persistence.query.journal.sql");
            
            var queries = AkkaExchangeQueries.Create(
                materializer, 
                readJournal);

            globalActorRefs.ErrorEventSubscriber = queries.HandlerErrorEventsSource;
            
            _system.AddDependencyResolver(
                new AutoFacDependencyResolver(container.Build(), _system));
            
            Queries = queries;

            _errorStreamLoggerSubscription = Queries.HandlerErrorEvents.Subscribe(e =>
                _logger.LogInformation(e.ToString()));
            
            _clientManager = _system.ActorOf(
                _system.DI().Props<ClientManagerActor>(), 
                "client-manager");
            globalActorRefs.ClientManager = _clientManager;

            _orderBook = _system.ActorOf(
                _system.DI().Props<OrderBookActor>(),
                "order-book");
            globalActorRefs.OrderBook = _orderBook;

            _orderExecutorManager = _system.ActorOf(
                _system.DI().Props<OrderExecutorManagerActor>(),
                "order-executor");
            globalActorRefs.OrderExecutorManager = _orderExecutorManager;

            // Match orders every second.
            _system
                .Scheduler
                .ScheduleTellRepeatedly(
                    TimeSpan.Zero,
                    TimeSpan.FromSeconds(1),
                    _orderBook,
                    new MatchOrdersCommand(),
                    ActorRefs.NoSender);
        }
        public void Dispose()
        {
            _system.Dispose();
            _logger.LogInformation("Disposed of AkkaExchange.");
        }

        public async Task<AkkaExchangeClient> NewConnection()
        {
            var command = new StartConnectionCommand();
            var persistenceId = command.ClientId.ToString();
            var inbox = Inbox.Create(_system);

            _clientManager.Tell(command, inbox.Receiver);

            var clientActor = await inbox.ReceiveAsync();
            var eventsQuery = Queries.Clients.Events(persistenceId);
            var stateQuery = Queries.Clients.State(persistenceId);
            var errorQuery = Queries.Clients.Errors(persistenceId);

            _logger.LogInformation($"Created new client {persistenceId}");
            
            return new AkkaExchangeClient(
                command.ClientId,
                inbox,
                clientActor as IActorRef,
                eventsQuery,
                stateQuery,
                errorQuery);
        }

        public async Task EndConnection(Guid connectionId)
        {
            var command = new EndConnectionCommand(connectionId);
            var responseObject = await _clientManager.Ask(command);
        }
    }
}