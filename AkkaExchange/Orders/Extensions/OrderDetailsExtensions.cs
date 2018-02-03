using System;

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
    }
}