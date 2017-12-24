using System;
using AkkaExchange.Utils;

namespace AkkaExchange.Client.Commands
{
    public class StartConnectionCommandHandler 
        : BaseCommandHandler<ClientState, StartConnectionCommand>
    {
        protected override HandlerResult Handle(ClientState state, StartConnectionCommand command)
        {
            throw new NotImplementedException();
        }
    }
}