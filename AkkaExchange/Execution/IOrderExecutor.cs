using System;
using AkkaExchange.Orders;

namespace AkkaExchange.Execution
{
    public interface IOrderExecutor
    {
        IObservable<OrderExecutionStatus> Execute(Order order);
    }
}
