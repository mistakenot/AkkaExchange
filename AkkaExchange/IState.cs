namespace AkkaExchange
{
    public interface IState<out TState>
    {
        TState Update(IEvent evnt);
    }
}
