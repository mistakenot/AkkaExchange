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
                if (state.Status != OrderExecutorStatus.Pending)
                {
                    return new HandlerResult(
                        $"Invalid starting state - {(int)state.Status} is not equal to {(int)OrderExecutorStatus.Pending}.");
                }

                return new HandlerResult(
                    new BeginOrderExecutionEvent(beginOrderExecutionCommand.Match));
            }

            if (command is UpdateOrderExecutionStatusCommand updateOrderExecutionStatusCommand)
            {
                if (updateOrderExecutionStatusCommand.OrderExecutionId != state.OrderExecutorId)
                {
                    return new HandlerResult($"Wrong client id.");
                }

                if (state.Status < updateOrderExecutionStatusCommand.Status)
                {
                    return new HandlerResult(
                        new UpdateOrderExecutionStatusEvent(
                            updateOrderExecutionStatusCommand.OrderExecutionId, 
                            updateOrderExecutionStatusCommand.Match,
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