using System;
using AkkaExchange.Orders;

namespace AkkaExchange.Execution.Commands
{
    public class UpdateOrderExecutionStatusCommand : Message, ICommand
    {
        public Guid OrderExecutionId { get; }
        public OrderMatch Match { get; }
        public OrderExecutorStatus Status { get; }

        public UpdateOrderExecutionStatusCommand(
            OrderExecutorStatus status,
            OrderMatch match,
            Guid orderExecutionId)
        {
            Match = match;
            Status = status;
            OrderExecutionId = orderExecutionId;
        }
    }
}