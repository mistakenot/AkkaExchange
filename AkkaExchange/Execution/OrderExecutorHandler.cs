using AkkaExchange.Execution.Commands;
using AkkaExchange.Execution.Events;
using AkkaExchange.Utils;

namespace AkkaExchange.Execution
{
    public class OrderExecutorHandler : ICommandHandler<OrderExecutorState>
    {
        public HandlerResult Handle(OrderExecutorState state, ICommand command)
        {
            if (command is BeginOrderExecutionCommand beginOrderExecutionCommand)
            {
                if (state.OrderId != beginOrderExecutionCommand.Order.OrderId)
                {
                    return new HandlerResult($"Wrong client id.");
                }

                return new HandlerResult(
                    new BeginOrderExecutionEvent(beginOrderExecutionCommand.Order));
            }

            return HandlerResult.NotHandled;
        }
    }
}