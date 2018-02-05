using System;
using Akka.Persistence.Query;
using Akka.Streams;
using Akka.Streams.Dsl;
using AkkaExchange.Orders.Events;
using AkkaExchange.Shared.Extensions;
using AkkaExchange.Utils;

namespace AkkaExchange.Orders.Queries
{
    public interface IPlacedOrderVolumeQueryFactory
    {
        IObservable<PlacedOrderVolume> Create(string persistenceId, TimeSpan timeChunkSize);
    }

    public class PlacedOrderVolumeQueryFactory : IPlacedOrderVolumeQueryFactory
    {
        private readonly IEventsByPersistenceIdQuery _eventsByPersistenceIdQuery;
        private readonly IMaterializer _materializer;

        public PlacedOrderVolumeQueryFactory(
            IEventsByPersistenceIdQuery eventsByPersistenceIdQuery,
            IMaterializer materializer)
        {
            _materializer = materializer ?? throw new ArgumentNullException(nameof(materializer));
            _eventsByPersistenceIdQuery = eventsByPersistenceIdQuery ?? throw new ArgumentNullException(nameof(eventsByPersistenceIdQuery));
        }

        public IObservable<PlacedOrderVolume> Create(string persistenceId, TimeSpan timeChunkSize)
        {
            var eventsSource = _eventsByPersistenceIdQuery.EventsByPersistenceId(
                    persistenceId,
                    0L,
                    long.MaxValue)
                .Where(env => env.Event is NewOrderEvent)
                .Select(env => env.Event as NewOrderEvent)
                .GroupedWithin(100, timeChunkSize)
                .Select(PlacedOrderVolume.FromEnumerable);
            
            return eventsSource.RunAsObservable(_materializer);
        }
    }
}