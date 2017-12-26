using AkkaExchange.Utils;

namespace AkkaExchange
{
    public interface ICommandHandler<in TState>
    {
        HandlerResult Handle(TState state, ICommand command);
    }
}