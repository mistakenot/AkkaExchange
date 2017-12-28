using System;

namespace AkkaExchange.Orders
{
    public class PlacedOrder : Order
    {
        public Guid OrderId { get; }
        public DateTime PlacedAt { get; }

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
            : base(details)
        {
            PlacedAt = placedAt;
            OrderId = orderId;
        }
    }
}