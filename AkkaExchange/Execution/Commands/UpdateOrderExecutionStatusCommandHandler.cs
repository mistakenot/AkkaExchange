using AkkaExchange.Execution.Events;
using AkkaExchange.Utils;

namespace AkkaExchange.Execution.Commands
{
    public class UpdateOrderExecutionStatusCommandHandler : 
        BaseCommandHandler<OrderExecutorState, UpdateOrderExecutionStatusCommand>
    {
        protected override HandlerResult Handle(OrderExecutorState state, UpdateOrderExecutionStatusCommand command)
        {
            if (state.OrderId != command.OrderId)
            {
                return new HandlerResult($"Order ID {command.OrderId} is incorrect.");
            }

            if (command.Status < state.Status ||
                command.Status == OrderExecutorStatus.Error && state.Status == OrderExecutorStatus.Complete)
            {
                return new HandlerResult($"Order status {command.Status} cannot come after {state.Status}.");
            }

            return new HandlerResult(
                new UpdateOrderExecutionStatusEvent(state.OrderId, command.Status));
        }
    }
}