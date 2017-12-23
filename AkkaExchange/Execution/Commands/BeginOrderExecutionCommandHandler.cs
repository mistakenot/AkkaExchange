using AkkaExchange.Execution.Events;

namespace AkkaExchange.Execution.Commands
{
    public class BeginOrderExecutionCommandHandler 
        : BaseCommandHandler<OrderExecutorManagerState, RequestOrderExecutionCommand>
    {
        protected override HandlerResult Handle(
            OrderExecutorManagerState managerState, 
            RequestOrderExecutionCommand command)
        {
            if (managerState.ExecutingObservables.ContainsKey(command.Order.OrderId))
            {
                return new HandlerResult($"Order ID {command.Order.OrderId} is already executing.");
            }

            return new HandlerResult(
                new BeginOrderExecutionEvent(command.Order));
        }
    }
}