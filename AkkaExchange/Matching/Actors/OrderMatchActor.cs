using AkkaExchange.Actors;
using AkkaExchange.State;

namespace AkkaExchange.Matching.Actors
{
    public class OrderMatchActor : BaseActor<MatchActorState>
    {
        public OrderMatchActor(
            ICommandHandler<MatchActorState> handler, 
            MatchActorState defaultState, 
            string persistenceId) 
            : base(handler, defaultState, persistenceId)
        {
        }
    }
}
