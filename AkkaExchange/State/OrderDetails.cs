using System;

namespace AkkaExchange.State
{
    public class Order
    {
        public Guid OrderId { get; }
        public OrderDetails Details { get; }

        public Order(OrderDetails details)
            : this(Guid.Empty, details)
        {
            
        }

        public Order(Guid orderId, OrderDetails details)
        {
            OrderId = orderId;
            Details = details ?? throw new ArgumentNullException(nameof(details));
        }
    }

    public class OrderDetails
    {
        public decimal Amount { get; }
        public decimal Price { get; }
        public OrderStateSide Side { get; }

        public OrderDetails(decimal amount, decimal price, OrderStateSide side)
        {
            if (amount < 0m) throw new ArgumentException("Amount must be > 0.");
            if (price < 0m) throw new ArgumentException("Price must be > 0.");

            Amount = amount;
            Price = price;
            Side = side;
        }

        public OrderDetails Add(decimal amount) =>
            WithAmount(Amount + amount);

        public OrderDetails Subtract(decimal amount) 
            => Add(-amount);

        public OrderDetails WithAmount(decimal amount) 
            => new OrderDetails(
                amount,
                Price,
                Side);
    }

    public enum OrderStateSide
    {
        Ask, Bid
    }
}
