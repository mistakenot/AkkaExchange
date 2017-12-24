using System;

namespace AkkaExchange.Execution.Commands
{
    public class UpdateOrderExecutionStatusCommand : ICommand
    {
        public OrderExecutorStatus Status { get; }
        public Guid OrderId { get; }

        public UpdateOrderExecutionStatusCommand(
            OrderExecutorStatus status, 
            Guid orderId)
        {
            Status = status;
            OrderId = orderId;
        }
    }
}