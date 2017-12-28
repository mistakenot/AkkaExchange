using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Akka.Persistence.Query;
using Akka.Persistence.Query.Sql;
using Akka.Streams;
using AkkaExchange.Client;
using AkkaExchange.Client.Actors;
using AkkaExchange.Client.Commands;
using AkkaExchange.Orders.Actors;
using AkkaExchange.Shared.Queries;
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

        public AkkaExchange(ContainerBuilder container, Config config)
        {
            _system = ActorSystem.Create("akka-exchange-system", config);

            _system.AddDependencyResolver(
                new AutoFacDependencyResolver(container.Build(), _system));

            var materializer = ActorMaterializer.Create(_system);
            var readJournal = PersistenceQuery.Get(_system).ReadJournalFor<SqlReadJournal>("akka.persistence.query.journal.sql");

            _clientStateQueryFactory = new StateQueryFactory<ClientState>(readJournal, materializer, ClientState.Empty);


            _source = readJournal.PersistenceIds().RunForeach(id =>
            {
                Console.WriteLine(id);
            }, materializer);
            
            _clientManager = _system.ActorOf(
                _system.DI().Props<ClientManagerActor>(), 
                "client-manager");

            _orderBook = _system.ActorOf(
                _system.DI().Props<OrderBookActor>(),
                "order-book");

            _eventsQueryFactory = new EventsQueryFactory(readJournal, materializer);

            Queries = AkkaExchangeQueries.Create(materializer, readJournal);
        }

        public void Dispose()
        {
            _system.Dispose();
        }

        public async Task<AkkaExchangeClient> NewConnection()
        {
            var command = new StartConnectionCommand();
            var inbox = Inbox.Create(_system);

            _clientManager.Tell(command, inbox.Receiver);

            var clientActor = await inbox.ReceiveAsync();
            var eventsQuery = _eventsQueryFactory.Create(command.ClientId.ToString());

            return new AkkaExchangeClient(
                command.ClientId,
                inbox,
                clientActor as IActorRef,
                eventsQuery);
        }

        public async Task EndConnection(Guid connectionId)
        {
            var command = new EndConnectionCommand(connectionId);
            var responseObject = await _clientManager.Ask(command);
        }
    }
}