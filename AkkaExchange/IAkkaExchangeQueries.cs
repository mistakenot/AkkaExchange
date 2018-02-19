using System;
using AkkaExchange.Client.Queries;
using AkkaExchange.Orders.Queries;
using AkkaExchange.Shared.Events;

namespace AkkaExchange
{
    public interface IAkkaExchangeQueries
    {
        IOrderQueries Orders { get; }
        IClientQueries Clients { get; }
        IObservable<HandlerErrorEvent> HandlerErrorEvents { get; }
    }
}