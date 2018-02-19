using System;
using System.Reactive.Linq;
using Akka.Persistence.Query;
using Akka.Streams;
using Akka.Streams.Dsl;
using AkkaExchange.Shared.Events;
using AkkaExchange.Utils;

namespace AkkaExchange.Client.Queries
{
    public class ClientQueries : IClientQueries
    {
        public IObservable<ClientManagerState> ClientManagerState { get; }

        private readonly Func<Guid, IObservable<HandlerResult>> _clientErrorFactory;
        private readonly Func<Guid, IObservable<IEvent>> _clientEventFactory;
        private readonly Func<Guid, IObservable<ClientState>> _clientStateFactory;

        public ClientQueries(
            IObservable<ClientManagerState> clientManagerState,
            Func<Guid, IObservable<HandlerResult>> clientErrorFactory,
            Func<Guid, IObservable<IEvent>> clientEventFactory)
        {
            ClientManagerState = clientManagerState ?? throw new ArgumentNullException(nameof(clientManagerState));
            _clientErrorFactory = clientErrorFactory ?? throw new ArgumentNullException(nameof(clientErrorFactory));
            _clientEventFactory = clientEventFactory ?? throw new ArgumentNullException(nameof(clientEventFactory));
        }

        public IObservable<HandlerResult> Errors(Guid clientId) => _clientErrorFactory(clientId);
        public IObservable<IEvent> Events(Guid clientId) => _clientEventFactory(clientId);
        public IObservable<ClientState> State(Guid clientId) => 
            Events(clientId).Scan(ClientState.Empty, (s, e) => s.Update(e));

        public static ClientQueries Create(
            string clientManagerPersistenceId,
            IEventsByPersistenceIdQuery eventsByPersistenceIdQuery,
            IMaterializer materializer,
            Source<HandlerErrorEvent, Akka.NotUsed> errorSource)
        {
            var clientManagerStateSource = eventsByPersistenceIdQuery
                .EventsByPersistenceId(clientManagerPersistenceId, 0L, long.MaxValue)
                .Where(e => e.Event is IEvent)
                .Select(e => e.Event as IEvent)
                .Scan(Client.ClientManagerState.Empty, (s, e) => s.Update(e));
            
            var clientManagerStateObservable = new SourceObservable<ClientManagerState>(
                clientManagerStateSource,
                materializer);

            Func<Guid, IObservable<IEvent>> clientEventFactory = (Guid g) => 
            {
                var source = eventsByPersistenceIdQuery
                    .EventsByPersistenceId(g.ToString(), 0L, long.MaxValue)
                    .Where(e => e.Event is IEvent)
                    .Select(e => e.Event as IEvent);
                
                return new SourceObservable<IEvent>(source, materializer);
            };

            Func<Guid, IObservable<HandlerResult>> clientErrorFactory = (Guid g) =>
            {
                var source = errorSource
                    .Where(e => e.Name == g.ToString())
                    .Select(e => e.Result);

                return new SourceObservable<HandlerResult>(
                    source,
                    materializer);
            };

            return new ClientQueries(
                clientManagerStateObservable,
                clientErrorFactory,
                clientEventFactory);
        }
    }
}