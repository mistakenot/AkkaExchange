using System;
using System.Linq;
using AkkaExchange.Client.Events;
using AkkaExchange.Utils;

namespace AkkaExchange.Client.Commands
{
    public class EndConnectionCommandHandler : BaseCommandHandler<ClientManagerState, EndConnectionCommand>
    {
        protected override HandlerResult Handle(ClientManagerState state, EndConnectionCommand command)
        {
            if (state.ClientIds.All(c => c != command.ClientId))
            {
                return new HandlerResult(
                    $"ClientId {command.ClientId} not found.");
            }

            return new HandlerResult(
                new EndConnectionEvent(
                    command.ClientId,
                    DateTime.UtcNow));
        }
    }
}