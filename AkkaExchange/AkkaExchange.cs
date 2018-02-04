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

namespace AkkaExchange
{
    public class AkkaExchange : IDisposable
    {
        public AkkaExchangeQueries Queries { get; }

        private readonly ActorSystem _system;
        private readonly IActorRef _clientManager;
        private readonly IActorRef _orderBook;
        private Task _source;
        private readonly IEventsQueryFactory _eventsQueryFactory;
        private readonly StateQueryFactory<ClientState> _clientStateQueryFactory;
        private readonly IActorRef _orderExecutorManager;

        public AkkaExchange(ContainerBuilder container, Config config)
        {
            _system = ActorSystem.Create("akka-exchange-system", config);

            var globalActorRefs = new GlobalActorRefs();

            container.RegisterInstance<IGlobalActorRefs>(globalActorRefs);

            var materializer = ActorMaterializer.Create(_system);
            var readJournal = PersistenceQuery.Get(_system).ReadJournalFor<SqlReadJournal>("akka.persistence.query.journal.sql");
            
            Queries = AkkaExchangeQueries.Create(
                materializer, 
                readJournal);

            globalActorRefs.ErrorEventSubscriber = Queries.HandlerErrorEventsSource;

            // Used in client actor
            container.RegisterInstance(Queries.OrderBookState);

            _system.AddDependencyResolver(
                new AutoFacDependencyResolver(container.Build(), _system));

            _clientStateQueryFactory = new StateQueryFactory<ClientState>(readJournal, materializer, ClientState.Empty);

            _source = readJournal.PersistenceIds().RunForeach(Console.WriteLine, materializer);
            
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

            _eventsQueryFactory = new EventsQueryFactory(readJournal, materializer);
        }
        public void Dispose()
        {
            _system.Dispose();
        }

        public async Task<AkkaExchangeClient> NewConnection()
        {
            var command = new StartConnectionCommand();
            var persistenceId = command.ClientId.ToString();
            var inbox = Inbox.Create(_system);

            _clientManager.Tell(command, inbox.Receiver);

            var clientActor = await inbox.ReceiveAsync();
            var eventsQuery = _eventsQueryFactory.Create(persistenceId);
            var stateQuery = _clientStateQueryFactory.Create(persistenceId);
            var errorQuery = Queries
                .HandlerErrorEvents
                .Select(e => e.Result);

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