using System;
using AkkaExchange.State;

namespace AkkaExchange.Commands
{
    public class AmendOrderCommand
    {
        public Guid OrderId { get; }
        public OrderDetails OrderDetails { get; }

        public AmendOrderCommand(Guid orderId, OrderDetails orderDetails)
        {
            OrderId = orderId;
            OrderDetails = orderDetails ?? throw new ArgumentNullException(nameof(orderDetails));
        }
    }
}