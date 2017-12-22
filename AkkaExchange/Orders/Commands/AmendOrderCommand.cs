using System;

namespace AkkaExchange.Orders.Commands
{
    public class AmendOrderCommand : IOrderCommand
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