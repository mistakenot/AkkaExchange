using System;

namespace AkkaExchange.Shared.Queries
{
    public interface IEventsQueryFactory
    {
        IObservable<object> Create(string persistenceId);
    }
}