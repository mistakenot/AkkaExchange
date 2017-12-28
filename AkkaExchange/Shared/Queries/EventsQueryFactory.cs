using System;
using Akka.Persistence.Query;
using Akka.Streams;
using Akka.Streams.Dsl;
using AkkaExchange.Utils;

namespace AkkaExchange.Shared.Queries
{
    public class EventsQueryFactory : IEventsQueryFactory
    {
        private readonly IEventsByPersistenceIdQuery _eventsByPersistenceIdQuery;
        private readonly IMaterializer _materializer;

        public EventsQueryFactory(
            IEventsByPersistenceIdQuery eventsByPersistenceIdQuery, 
            IMaterializer materializer)
        {
            _materializer = materializer ?? throw new ArgumentNullException(nameof(materializer));
            _eventsByPersistenceIdQuery = eventsByPersistenceIdQuery ?? throw new ArgumentNullException(nameof(eventsByPersistenceIdQuery));
        }

        public IObservable<object> Create(string persistenceId)
        {
            var eventsSource = _eventsByPersistenceIdQuery.EventsByPersistenceId(
                    persistenceId,
                    0L,
                    long.MaxValue)
                .Select(env => env.Event);

            return new SourceObservable<object>(eventsSource, _materializer);
        }
    }
}