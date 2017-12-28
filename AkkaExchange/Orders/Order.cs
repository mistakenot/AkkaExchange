using System;

namespace AkkaExchange.Orders
{
    public class Order
    {
        public decimal Amount { get; }
        public decimal Price { get; }
        public OrderSide Side { get; }
        public Guid ClientId { get; }
        public Guid OrderId { get; }
        public DateTime PlacedAt { get; }

        public Order(Guid clientId, decimal amount, decimal price, OrderSide side)
            : this(clientId, Guid.NewGuid(), amount, price, side, DateTime.UtcNow)
        {
            
        }

        public Order(Guid clientId, Guid orderId, decimal amount, decimal price, OrderSide side, DateTime placedAt)
        {
            if (amount < 0m) throw new ArgumentException("Amount must be > 0.");
            if (price < 0m) throw new ArgumentException("Price must be > 0.");

            Amount = amount;
            Price = price;
            Side = side;
            PlacedAt = placedAt;
            ClientId = clientId;
            OrderId = orderId;
        }
    }
}
