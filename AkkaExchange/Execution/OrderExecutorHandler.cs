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

            if (command is UpdateOrderExecutionStatusCommand updateOrderExecutionStatusCommand)
            {
                if (updateOrderExecutionStatusCommand.OrderId != state.OrderId)
                {
                    return new HandlerResult($"Wrong client id.");
                }

                if (state.Status < updateOrderExecutionStatusCommand.Status)
                {
                    return new HandlerResult(
                        new UpdateOrderExecutionStatusEvent(
                            updateOrderExecutionStatusCommand.OrderId, 
                            updateOrderExecutionStatusCommand.Status));
                }
                else
                {
                    return new HandlerResult(
                        $"Invalid state change: {state.Status} -> {updateOrderExecutionStatusCommand.Status}");
                }
            }

            return HandlerResult.NotHandled(command);
        }
    }
}