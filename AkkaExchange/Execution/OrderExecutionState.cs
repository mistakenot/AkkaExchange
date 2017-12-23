using System;
using AkkaExchange.Execution.Events;
using AkkaExchange.State;

namespace AkkaExchange.Execution
{
    public class OrderExecutionState : IState<OrderExecutionState>
    {
        public Guid OrderId { get; }
        public OrderExecutionStatus Status { get; }
        public IOrderExecutionObservable Observable { get; }

        public OrderExecutionState(
            Guid orderId,
            IOrderExecutionObservable observable)
            : this(orderId, OrderExecutionStatus.Pending, observable)
        {
        }

        public OrderExecutionState(
            Guid orderId, 
            OrderExecutionStatus status,
            IOrderExecutionObservable observable)
        {
            OrderId = orderId;
            Status = status;
            Observable = observable ?? throw new ArgumentNullException(nameof(observable));
        }
        public OrderExecutionState Update(IEvent evnt)
        {
            if (evnt is BeginOrderExecutionEvent beginOrderExecutionEvent)
            {
                return new OrderExecutionState(
                    OrderId,
                    OrderExecutionStatus.InProgress,
                    Observable);
            }

            return this;
        }
    }
}
