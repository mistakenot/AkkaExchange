using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Akka.Persistence.Query;
using Akka.Persistence.Query.Sql;
using Akka.Streams;
using Akka.Streams.Dsl;
using AkkaExchange.Client.Actors;
using AkkaExchange.Client.Commands;
using AkkaExchange.Orders.Actors;
using Autofac;

namespace AkkaExchange
{
    public class AkkaExchange : IDisposable
    {
        private readonly ActorSystem _system;
        private readonly SqlReadJournal _readJournal;
        private readonly IActorRef _clientManager;
        private readonly IActorRef _orderBook;
        private readonly ActorMaterializer _materializer;
        private Task _source;

        public AkkaExchange(ContainerBuilder container, Config config)
        {
            _system = ActorSystem.Create("akka-exchange-system", config);

            _system.AddDependencyResolver(
                new AutoFacDependencyResolver(container.Build(), _system));

            _materializer = ActorMaterializer.Create(_system);
            _readJournal = PersistenceQuery.Get(_system).ReadJournalFor<SqlReadJournal>("akka.persistence.query.journal.sql");
            
            _source = _readJournal.PersistenceIds().RunForeach(id =>
            {
                Console.WriteLine(id);
            }, _materializer);
            
            _clientManager = _system.ActorOf(
                _system.DI().Props<ClientManagerActor>(), 
                "client-manager");

            _orderBook = _system.ActorOf(
                _system.DI().Props<OrderBookActor>(),
                "order-book");
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

            var eventsSource = _readJournal.EventsByPersistenceId(
                command.ClientId.ToString(),
                0L,
                long.MaxValue);

            var subscription = eventsSource
                .Select(env => env.Event)
                .RunForeach(e =>
                {
                    inbox.Receiver.Tell(e);
                }, _materializer);

            return new AkkaExchangeClient(
                command.ClientId,
                clientActor as IActorRef,
                subscription,
                inbox);
        }

        public async Task EndConnection(Guid connectionId)
        {
            var command = new EndConnectionCommand(connectionId);
            var responseObject = await _clientManager.Ask(command);
        }
    }
}