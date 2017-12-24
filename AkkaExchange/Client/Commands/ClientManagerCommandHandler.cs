namespace AkkaExchange.Client.Commands
{
    public class ClientManagerCommandHandler : CommandHandlerCollection<ClientManagerState>
    {
        public ClientManagerCommandHandler() : base(
                new ICommandHandler<ClientManagerState>[]
                {
                    new StartConnectionCommandHandler(), 
                    new EndConnectionCommandHandler(), 
                })
        {
        }
    }
}