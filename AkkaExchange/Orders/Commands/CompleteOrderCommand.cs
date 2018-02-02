using System;

namespace AkkaExchange.Orders.Commands
{
    public class CompleteOrderCommand : Message, ICommand
    {
        public Guid OrderId { get; }

        public CompleteOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}