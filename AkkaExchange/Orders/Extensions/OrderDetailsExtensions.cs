namespace AkkaExchange.Orders.Extensions
{
    public static class OrderDetailsExtensions
    {
        public static OrderDetails Add(this OrderDetails order, decimal amount) =>
            WithAmount(order, order.Amount + amount);

        public static OrderDetails Subtract(this OrderDetails order, decimal amount)
            => Add(order, -amount);

        public static OrderDetails WithAmount(this OrderDetails order, decimal amount)
            => new OrderDetails(
                amount,
                order.Price,
                order.Side);
    }
}