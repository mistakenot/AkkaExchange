namespace AkkaExchange.State
{
    public interface IState<out TState>
    {
        TState Update(IEvent evnt);
    }
}
