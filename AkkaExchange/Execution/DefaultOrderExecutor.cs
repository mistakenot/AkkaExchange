using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using AkkaExchange.Orders;

namespace AkkaExchange.Execution
{
    public class DefaultOrderExecutor : IOrderExecutor
    {
        public IObservable<OrderExecutionStatus> Execute(Order order)
        {
            var observer = new Subject<OrderExecutionStatus>();

            Task.Run(async () =>
            {
                observer.OnNext(OrderExecutionStatus.Pending);

                // Simulate processing an order.
                await Task.Delay(100);
                observer.OnNext(OrderExecutionStatus.InProgress);

                await Task.Delay(250);
                observer.OnNext(OrderExecutionStatus.Complete);
            });

            return observer;
        }
    }
}