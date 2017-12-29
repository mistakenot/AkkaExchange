using System;

namespace AkkaExchange.Orders.Commands
{
    public class CompleteOrderCommand : ICommand
    {
        public Guid OrderId { get; }

        public CompleteOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}