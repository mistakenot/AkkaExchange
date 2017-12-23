using AkkaExchange.Execution.Events;

namespace AkkaExchange.Execution.Commands
{
    public class UpdateOrderExecutionStatusCommandHandler : 
        BaseCommandHandler<OrderExecutionState, UpdateOrderExecutionStatusCommand>
    {
        protected override HandlerResult Handle(OrderExecutionState state, UpdateOrderExecutionStatusCommand command)
        {
            if (state.OrderId != command.OrderId)
            {
                return new HandlerResult($"Order ID {command.OrderId} is incorrect.");
            }

            if (command.Status < state.Status ||
                command.Status == OrderExecutionStatus.Error && state.Status == OrderExecutionStatus.Complete)
            {
                return new HandlerResult($"Order status {command.Status} cannot come after {state.Status}.");
            }

            return new HandlerResult(
                new UpdateOrderExecutionStatusEvent(state.OrderId, command.Status));
        }
    }
}