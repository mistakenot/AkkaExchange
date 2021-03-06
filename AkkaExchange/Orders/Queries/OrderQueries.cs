using System;
using System.Linq;
using Akka.Persistence.Query;
using Akka.Streams;
using Akka.Streams.Dsl;
using AkkaExchange.Orders.Events;
using AkkaExchange.Utils;

namespace AkkaExchange.Orders.Queries
{
    public class OrderQueries : IOrderQueries
    {
        public IObservable<OrderBookState> OrderBookState { get; }
        public IObservable<IEvent> OrderBookEvents { get; }

        public OrderQueries(
            IObservable<OrderBookState> orderBookState,
            IObservable<IEvent> orderBookEvents)
        {
            OrderBookEvents = orderBookEvents;
            OrderBookState = orderBookState;
        }

        public static OrderQueries Create(
            string orderBookPersistenceId,
            IEventsByPersistenceIdQuery eventsByPersistenceIdQuery,
            IMaterializer materializer)
        {
            var orderBookSource = eventsByPersistenceIdQuery
                .EventsByPersistenceId(orderBookPersistenceId, 0, long.MaxValue)
                .Where(e => e.Event is IEvent)
                .Select(e => e.Event as IEvent)
                .Where(e => !(e is MatchedOrdersEvent matchedOrdersEvent && 
                              !matchedOrdersEvent.MatchedOrders.Matches.Any()));
            
            var orderBookEventsObservable = new SourceObservable<IEvent>(
                orderBookSource, 
                materializer);

            var orderBookStateSource = orderBookSource
                .Scan(
                    Orders.OrderBookState.Empty, 
                    (s, e) => s.Update(e));
            
            var orderBookStateObservable = new SourceObservable<Orders.OrderBookState>(
                orderBookStateSource, 
                materializer);

            return new OrderQueries(orderBookStateObservable, orderBookEventsObservable);
        }
    }
}