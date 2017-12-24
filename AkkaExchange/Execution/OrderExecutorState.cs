using System;
using AkkaExchange.Execution.Events;

namespace AkkaExchange.Execution
{
    public class OrderExecutorState : IState<OrderExecutorState>
    {
        public Guid OrderId { get; }
        public OrderExecutorStatus Status { get; }

        public OrderExecutorState(Guid orderId)
            : this(orderId, OrderExecutorStatus.Pending)
        {
            
        }

        public OrderExecutorState(
            Guid orderId, 
            OrderExecutorStatus status)
        {
            OrderId = orderId;
            Status = status;
        }

        public OrderExecutorState Update(IEvent evnt)
        {
            if (evnt is BeginOrderExecutionEvent beginOrderExecutionEvent && 
                beginOrderExecutionEvent.Order.OrderId == OrderId)
            {
                return new OrderExecutorState(
                    OrderId,
                    OrderExecutorStatus.InProgress);
            }

            return this;
        }
    }
}
