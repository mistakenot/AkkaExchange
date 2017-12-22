using AkkaExchange.Events;
using AkkaExchange.State;

namespace AkkaExchange.Handlers
{
    public class MatchActorCommandHandler : ICommandHandler<MatchActorState>
    {
        public HandlerResult Handle(MatchActorState state, object command)
        {
            switch (command)
            {
                case BeginMatchOrdersEvent beginMatchOrdersEvent:
                    

                    return HandlerResult.NotFound(command);
                default:
                    return HandlerResult.NotFound(command);

            }
        }
    }
}
