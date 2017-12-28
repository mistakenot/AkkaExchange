using System;

namespace AkkaExchange.Orders.Extensions
{
    public static class OrderDetailsExtensions
    {
        public static PlacedOrder WithAmount(this PlacedOrder order, decimal amount)
            => new PlacedOrder(
                new Order(
                    order.ClientId,
                    amount,
                    order.Price,
                    order.Side),
                order.PlacedAt,
                order.OrderId);
        
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