using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace AkkaExchange.Orders.Extensions
{
    public static class OrderDetailsExtensions
    {
        // TODO this is all a bit icky. Doesn't inheritence suck?
        public static PlacedOrder WithAmount(this PlacedOrder order, decimal amount)
            => new PlacedOrder(
                (order as Order).WithAmount(amount),
                order.PlacedAt,
                order.OrderId);
        
        public static Order WithAmount(this Order order, decimal amount)
            => new Order(
                    order.ClientId,
                    amount,
                    order.Price,
                    order.Side);
        public static PlacedOrder WithPrice(this PlacedOrder order, decimal price)
            => new PlacedOrder(
                new Order(
                    order.ClientId,
                    order.Amount,
                    price,
                    order.Side),
                order.PlacedAt,
                order.OrderId);

        public static IObservable<ILookup<Guid, PlacedOrder>> CompleteOrdersGroupedByClientId(this IObservable<OrderBookState> observable) =>
            observable.Select(book =>
                book.CompleteOrders.ToLookup(o => o.ClientId));

        public static IObservable<ILookup<Guid, PlacedOrder>> IncompleteOrdersGroupedByClientId(this IObservable<OrderBookState> observable) =>
            observable.Select(book =>
                book.OpenOrders.Concat(book.ExecutingOrders).ToLookup(o => o.ClientId));

        public static IObservable<IEnumerable<PlacedOrder>> CompleteOrders(this IObservable<OrderBookState> observable) =>
            observable.Select(book => book.CompleteOrders).DistinctUntilChanged(orders => orders.Count);
    }
}