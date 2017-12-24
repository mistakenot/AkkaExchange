using AkkaExchange.Utils;

namespace AkkaExchange.Client.Commands
{
    public class ClientCommandHandler : ICommandHandler<ClientState>
    {
        public bool CanHandle(object command)
        {
            throw new System.NotImplementedException();
        }

        public HandlerResult Handle(ClientState state, object command)
        {
            throw new System.NotImplementedException();
        }
    }
}