using System;
using Akka.Persistence.Query;
using Akka.Streams;
using Akka.Streams.Dsl;
using AkkaExchange.Shared.Extensions;
using AkkaExchange.Utils;

namespace AkkaExchange.Shared.Queries
{
    class StateQueryFactory<T> : IStateQueryFactory<T>
        where T : IState<T>
    {
        private readonly T _defaultState;
        private readonly IEventsByPersistenceIdQuery _eventsByPersistenceIdQuery;
        private readonly IMaterializer _materializer;

        public StateQueryFactory(
            IEventsByPersistenceIdQuery eventsByPersistenceIdQuery,
            IMaterializer materializer,
            T defaultState)
        {
            _defaultState = defaultState;
            _materializer = materializer ?? throw new ArgumentNullException(nameof(materializer));
            _eventsByPersistenceIdQuery = eventsByPersistenceIdQuery ?? throw new ArgumentNullException(nameof(eventsByPersistenceIdQuery));
        }

        public IObservable<T> Create(string persistenceId)
        {
            var source = _eventsByPersistenceIdQuery.EventsByPersistenceId(
                    persistenceId,
                    0,
                    long.MaxValue)
                .Select(e => e.Event)
                .Where(e => e is IEvent)
                .Scan(_defaultState, (state, evnt) => state.Update(evnt as IEvent));

            return source.RunAsObservable(_materializer);
        }
    }
}