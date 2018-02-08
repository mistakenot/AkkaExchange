using AkkaExchange.Execution.Commands;
using AkkaExchange.Execution.Events;
using AkkaExchange.Utils;

namespace AkkaExchange.Execution
{
    public class OrderExecutorManagerHandler : ICommandHandler<OrderExecutorManagerState>
    {
        public HandlerResult Handle(OrderExecutorManagerState state, ICommand command)
        {
            if (command is BeginOrderExecutionCommand beginOrderExecutionCommand)
            {
                if (state.ExecutingObservables.ContainsKey(beginOrderExecutionCommand.Order.OrderId))
                {
                    return new HandlerResult($"Order ID {beginOrderExecutionCommand.Order.OrderId} is already executing.");
                }

                return new HandlerResult(
                    new BeginOrderExecutionEvent(
                        beginOrderExecutionCommand.Order));
            }

            return HandlerResult.NotHandled(command);
        }
    }
}