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
                return new HandlerResult(
                    new BeginOrderExecutionEvent(
                        beginOrderExecutionCommand.Match));
            }

            return HandlerResult.NotHandled(command);
        }
    }
}