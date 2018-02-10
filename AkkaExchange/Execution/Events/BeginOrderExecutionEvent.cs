using AkkaExchange.Orders;
using System;

namespace AkkaExchange.Execution.Events
{
    public class BeginOrderExecutionEvent : Message, IEvent
    {
        public Guid OrderExecutionId { get; }
        public OrderMatch Match { get; }

        public BeginOrderExecutionEvent(OrderMatch match)
            : this (Guid.NewGuid(), match)
        {
            
        }

        public BeginOrderExecutionEvent(
            Guid orderExecutionId, 
            OrderMatch match)
        {
            Match = match ?? throw new ArgumentNullException(nameof(match));
        }
    }
}