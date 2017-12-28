using System;

namespace AkkaExchange.Shared.Queries
{
    public interface IStateQueryFactory<out T>
        where T : IState<T>
    {
        IObservable<T> Create(string persistenceId);
    }
}

