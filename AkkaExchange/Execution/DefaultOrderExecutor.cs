using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using AkkaExchange.Orders;

namespace AkkaExchange.Execution
{
    public class DefaultOrderExecutor : IOrderExecutor
    {
        public IObservable<OrderExecutorStatus> Execute(Order order)
        {
            var observer = new Subject<OrderExecutorStatus>();

            Task.Run(async () =>
            {
                observer.OnNext(OrderExecutorStatus.Pending);

                // Simulate processing an order.
                await Task.Delay(100);
                observer.OnNext(OrderExecutorStatus.InProgress);

                await Task.Delay(250);
                observer.OnNext(OrderExecutorStatus.Complete);
            });

            return observer;
        }
    }
}