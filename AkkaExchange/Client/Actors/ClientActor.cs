using AkkaExchange.Actors;

namespace AkkaExchange.Client.Actors
{
    public class ClientActor : BaseActor<ClientState>
    {
        public ClientActor(
            ICommandHandler<ClientState> handler, 
            ClientState defaultState, 
            string persistenceId) : base(handler, defaultState, persistenceId)
        {
        }
    }
}