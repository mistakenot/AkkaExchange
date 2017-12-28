using System;
using System.Linq;
using AkkaExchange.Client.Commands;
using AkkaExchange.Client.Events;
using AkkaExchange.Execution.Events;
using AkkaExchange.Utils;

namespace AkkaExchange.Client
{
    public class ClientManagerHandler : ICommandHandler<ClientManagerState>
    {
        public HandlerResult Handle(ClientManagerState state, ICommand command)
        {
            if (command is StartConnectionCommand startConnectionCommand)
            {
                if (state.ClientIds.Any(id => id == startConnectionCommand.ClientId))
                {
                    return new HandlerResult(
                        $"Client Id {startConnectionCommand.ClientId} already exists.");
                }

                return new HandlerResult(
                    new StartConnectionEvent(
                        startConnectionCommand.ClientId,
                        DateTime.UtcNow));
            }

            if (command is EndConnectionCommand endConnectionCommand)
            {
                if (state.ClientIds.Contains(endConnectionCommand.ClientId))
                {
                    return new HandlerResult(
                        new EndConnectionEvent(endConnectionCommand.ClientId));
                }
                else
                {
                    return new HandlerResult(
                        $"Connection Id {endConnectionCommand.ClientId} not found.");
                }
            }

            if (command is ExecuteOrderCommand executeOrderCommand)
            {
                if (state.ClientIds.Contains(executeOrderCommand.ClientId))
                {
                    return new HandlerResult(
                        new ExecuteOrderEvent(
                            executeOrderCommand.ClientId, 
                            executeOrderCommand.OrderCommand));
                }
                else
                {
                    return new HandlerResult(
                        $"Connection Id {executeOrderCommand.ClientId} not found.");
                }
            }

            return HandlerResult.NotHandled;
        }
    }
}