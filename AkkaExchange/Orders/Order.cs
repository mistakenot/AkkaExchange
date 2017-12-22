using System;

namespace AkkaExchange.Orders
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
}