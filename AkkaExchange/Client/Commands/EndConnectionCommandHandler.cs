using System;
using AkkaExchange.Utils;

namespace AkkaExchange.Client.Commands
{
    public class EndConnectionCommandHandler : BaseCommandHandler<ClientState, EndConnectionCommand>
    {
        protected override HandlerResult Handle(ClientState state, EndConnectionCommand command)
        {
            throw new NotImplementedException();
        }
    }
}