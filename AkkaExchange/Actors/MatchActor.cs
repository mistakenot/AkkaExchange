using AkkaExchange.Handlers;
using AkkaExchange.State;

namespace AkkaExchange.Actors
{
    public class MatchActor : BaseActor<MatchActorState, MatchActorCommandHandler>
    {
        public MatchActor(
            MatchActorCommandHandler handler, 
            MatchActorState state) 
            : base(
                  handler, 
                  state, 
                  "match-actor")
        {
        }
    }
}
