using System;

namespace AkkaExchange.Orders
{
    public class PlacedOrder
    {
        public Order Details { get; }
        public DateTime PlacedAt { get; }

        public PlacedOrder(
            Order details, 
            DateTime placedAt)
        {
            Details = details ?? throw new ArgumentNullException(nameof(details));
            PlacedAt = placedAt;
        }
    }
}