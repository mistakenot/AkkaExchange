using System;
using AkkaExchange.Client.Events;
using AkkaExchange.Utils;

namespace AkkaExchange.Client.Commands
{
    public class StartConnectionCommandHandler : BaseCommandHandler<ClientManagerState, StartConnectionCommand>
    {
        protected override HandlerResult Handle(ClientManagerState state, StartConnectionCommand command)
        {
            return new HandlerResult(
                new StartConnectionEvent(
                    Guid.NewGuid(),
                    DateTime.UtcNow));
        }
    }

    public class ClientActorStartConnectionCommandHandler : BaseCommandHandler<ClientState, StartConnectionCommand>
    {
        protected override HandlerResult Handle(ClientState state, StartConnectionCommand command)
        {
            if (state.Status != ClientStatus.Pending)
            {
                return new HandlerResult(
                    $"Client actor is in invalid state {state.Status}. Cannot start connection.");
            }

            return new HandlerResult(
                new StartConnectionEvent(
                    state.ClientId,
                    DateTime.UtcNow));
        }
    }
}