using System;
using Akka.Persistence.Query;
using Akka.Streams;
using Akka.Streams.Dsl;
using AkkaExchange.Orders.Events;
using AkkaExchange.Shared.Extensions;
using AkkaExchange.Utils;

namespace AkkaExchange.Orders.Queries
{
    public interface IOrderEventsByClientQueryFactory
    {
        IObservable<IEvent> Create(Guid clientId);
    }

    public class OrderEventsByClientQueryFactory : IOrderEventsByClientQueryFactory
    {
        private readonly IEventsByPersistenceIdQuery _eventsByPersistenceIdQuery;
        private readonly IMaterializer _materializer;
        private readonly string _orderBookPersistenceId;

        public OrderEventsByClientQueryFactory(
            IEventsByPersistenceIdQuery eventsByPersistenceIdQuery,
            IMaterializer materializer,
            string orderBookPersistenceId)
        {
            _eventsByPersistenceIdQuery = eventsByPersistenceIdQuery ?? throw new ArgumentNullException(nameof(eventsByPersistenceIdQuery));
            _materializer = materializer ?? throw new ArgumentNullException(nameof(materializer));
            _orderBookPersistenceId = orderBookPersistenceId ?? throw new ArgumentNullException(nameof(orderBookPersistenceId));
        }

        public IObservable<IEvent> Create(Guid clientId)
        {
            var source = _eventsByPersistenceIdQuery.EventsByPersistenceId(
                _orderBookPersistenceId,
                0L,
                long.MaxValue)
                .Where(e => e.Event is IClientOrderEvent)
                .Select(e => e.Event as IClientOrderEvent)
                .Where(e => e.ClientId == clientId);

            return source.RunAsObservable(_materializer);
        }
    }
}