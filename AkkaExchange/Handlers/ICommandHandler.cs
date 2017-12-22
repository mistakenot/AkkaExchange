namespace AkkaExchange.Handlers
{
    public interface ICommandHandler<in TState>
    {
        HandlerResult Handle(TState state, object command);
    }
}