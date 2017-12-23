using System;

namespace AkkaExchange.Orders.Extensions
{
    public static class OrderDetailsExtensions
    {
        public static Order Add(this Order order, decimal amount) =>
            WithAmount(order, order.Amount + amount);

        public static Order Subtract(this Order order, decimal amount)
            => Add(order, -amount);

        public static Order WithAmount(this Order order, decimal amount)
            => new Order(
                order.ClientId,
                order.OrderId,
                amount,
                order.Price,
                order.Side);

        public static Order WithOrderId(this Order order, Guid orderId)
            => new Order(
                order.ClientId,
                orderId, 
                order.Amount,
                order.Price,
                order.Side);

        public static (Order, Order) Split(this Order order, decimal amount)
        {
            if (amount > order.Amount)
            {
                throw new ArgumentException("Amount is invalid.");
            }

            return (
                order.WithAmount(amount),
                order.WithAmount(order.Amount - amount));
        }
    }
}