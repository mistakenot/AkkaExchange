using System;

namespace AkkaExchange.Commands
{
    public class RemoveOrderCommand
    {
        public Guid OrderId { get; }

        public RemoveOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
