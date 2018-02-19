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

        private readonly Func<string, IObservable<HandlerResult>> _clientErrorFactory;
        private readonly Func<string, IObservable<IEvent>> _clientEventFactory;
        private readonly Func<string, IObservable<ClientState>> _clientStateFactory;

        public ClientQueries(
            IObservable<ClientManagerState> clientManagerState,
            Func<string, IObservable<HandlerResult>> clientErrorFactory,
            Func<string, IObservable<IEvent>> clientEventFactory)
        {
            ClientManagerState = clientManagerState ?? throw new ArgumentNullException(nameof(clientManagerState));
            _clientErrorFactory = clientErrorFactory ?? throw new ArgumentNullException(nameof(clientErrorFactory));
            _clientEventFactory = clientEventFactory ?? throw new ArgumentNullException(nameof(clientEventFactory));
        }

        public IObservable<HandlerResult> Errors(string persistenceId) => _clientErrorFactory(persistenceId);
        public IObservable<IEvent> Events(string persistenceId) => _clientEventFactory(persistenceId);
        public IObservable<ClientState> State(string persistenceId) => 
            Events(persistenceId).Scan(ClientState.Empty, (s, e) => s.Update(e));

        public static ClientQueries Create(
            string clientManagerPersistenceId,
            IEventsByPersistenceIdQuery eventsByPersistenceIdQuery,
            IMaterializer materializer,
            IObservable<HandlerErrorEvent> handlerErrorEvents)
        {
            var clientManagerStateSource = eventsByPersistenceIdQuery
                .EventsByPersistenceId(clientManagerPersistenceId, 0L, long.MaxValue)
                .Where(e => e.Event is IEvent)
                .Select(e => e.Event as IEvent)
                .Scan(Client.ClientManagerState.Empty, (s, e) => s.Update(e));
            
            var clientManagerStateObservable = new SourceObservable<ClientManagerState>(
                clientManagerStateSource,
                materializer);

            Func<string, IObservable<IEvent>> clientEventFactory = (string id) => 
            {
                var source = eventsByPersistenceIdQuery
                    .EventsByPersistenceId(id.ToString(), 0L, long.MaxValue)
                    .Where(e => e.Event is IEvent)
                    .Select(e => e.Event as IEvent);
                
                return new SourceObservable<IEvent>(source, materializer);
            };

            Func<string, IObservable<HandlerResult>> clientErrorFactory = (string id) =>
                handlerErrorEvents.Where(e => e.Name == id.ToString()).Select(e => e.Result);
            

            return new ClientQueries(
                clientManagerStateObservable,
                clientErrorFactory,
                clientEventFactory);
        }
    }
}