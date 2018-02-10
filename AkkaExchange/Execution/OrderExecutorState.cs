using System;
using AkkaExchange.Execution.Events;
using AkkaExchange.Orders;

namespace AkkaExchange.Execution
{
    public class OrderExecutorState : Message, IState<OrderExecutorState>
    {
        public Guid OrderExecutorId { get; }
        public OrderMatch Match { get; }
        public OrderExecutorStatus Status { get; }

        public OrderExecutorState(OrderMatch orderMatch)
            : this(
                Guid.NewGuid(), 
                orderMatch, 
                OrderExecutorStatus.Pending)
        {
            
        }

        public OrderExecutorState(
            Guid orderExecutorId,
            OrderMatch orderMatch,
            OrderExecutorStatus status)
        {
            OrderExecutorId = orderExecutorId;
            Match = orderMatch;
            Status = status;
        }

        public OrderExecutorState Update(IEvent evnt)
        {
            if (evnt is BeginOrderExecutionEvent beginOrderExecutionEvent)
            {
                return new OrderExecutorState(
                    beginOrderExecutionEvent.OrderExecutionId,
                    beginOrderExecutionEvent.Match,
                    OrderExecutorStatus.Pending);
            }

            return this;
        }
    }
}