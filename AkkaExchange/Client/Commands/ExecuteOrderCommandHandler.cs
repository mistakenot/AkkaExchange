using System;
using AkkaExchange.Utils;

namespace AkkaExchange.Client.Commands
{
    public class ExecuteOrderCommandHandler : BaseCommandHandler<ClientState, ExecuteOrderCommand>
    {
        protected override HandlerResult Handle(ClientState state, ExecuteOrderCommand command)
        {
            throw new NotImplementedException();
        }
    }
}