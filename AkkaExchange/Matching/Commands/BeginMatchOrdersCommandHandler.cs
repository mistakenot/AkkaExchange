using AkkaExchange.Matching.Events;
using AkkaExchange.State;
using AkkaExchange.Utils;

namespace AkkaExchange.Matching.Commands
{
    public class BeginMatchOrdersCommandHandler : BaseCommandHandler<ExchangeActorState, BeginMatchOrdersCommand>
    {
        protected override HandlerResult Handle(ExchangeActorState state, BeginMatchOrdersCommand command)
        {
            return new HandlerResult(
                new BeginMatchOrdersEvent(
                    state.Orders));
        }
    }
}