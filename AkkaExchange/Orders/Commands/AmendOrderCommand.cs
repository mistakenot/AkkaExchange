using System;

namespace AkkaExchange.Orders.Commands
{
    public class AmendOrderCommand : Message, ICommand
    {
        public Guid OrderId { get; }
        public Order Order { get; }

        public AmendOrderCommand(Guid orderId, Order order)
        {
            OrderId = orderId;
            Order = order ?? throw new ArgumentNullException(nameof(order));
        }
    }
}