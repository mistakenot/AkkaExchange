using System;

namespace AkkaExchange.Execution.Commands
{
    public class UpdateOrderExecutionStatusCommand : ICommand
    {
        public OrderExecutionStatus Status { get; }
        public Guid OrderId { get; }

        public UpdateOrderExecutionStatusCommand(
            OrderExecutionStatus status, 
            Guid orderId)
        {
            Status = status;
            OrderId = orderId;
        }
    }
}