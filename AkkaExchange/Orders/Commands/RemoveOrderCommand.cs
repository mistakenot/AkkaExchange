using System;

namespace AkkaExchange.Orders.Commands
{
    public class RemoveOrderCommand : ICommand
    {
        public Guid OrderId { get; }

        public RemoveOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}