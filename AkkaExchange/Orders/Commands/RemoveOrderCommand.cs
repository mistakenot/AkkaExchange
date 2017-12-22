using System;

namespace AkkaExchange.Orders.Commands
{
    public class RemoveOrderCommand : IOrderCommand
    {
        public Guid OrderId { get; }

        public RemoveOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
