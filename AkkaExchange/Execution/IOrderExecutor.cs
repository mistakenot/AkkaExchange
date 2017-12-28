using System;
using AkkaExchange.Orders;

namespace AkkaExchange.Execution
{
    public interface IOrderExecutor
    {
        IObservable<OrderExecutorStatus> Execute(PlacedOrder order);
    }
}