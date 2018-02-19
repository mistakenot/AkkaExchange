using System;

namespace AkkaExchange.Orders.Queries
{
    public interface IOrderQueries
    {
        IObservable<OrderBookState> OrderBookState { get; }

        IObservable<IEvent> OrderBookEvents { get; }
    }
}