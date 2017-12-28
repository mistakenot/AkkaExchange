using System;

namespace AkkaExchange.Orders
{
    public class PlacedOrder 
    {
        public Order Details { get; }
        public DateTime PlacedAt { get; }
        public Guid OrderId { get; }

        public PlacedOrder(
            Order order)
            : this(order, DateTime.UtcNow, Guid.NewGuid())
        {
            
        }

        public PlacedOrder(
            Order details,
            DateTime placedAt)
            : this(details, placedAt, Guid.NewGuid())
        {
            
        }

        public PlacedOrder(
            Order details, 
            DateTime placedAt, 
            Guid orderId)
        {
            Details = details ?? throw new ArgumentNullException(nameof(details));
            PlacedAt = placedAt;
            OrderId = orderId;
        }
    }
}