using System;

namespace AkkaExchange.Execution.Events
{
    public class UpdateOrderExecutionStatusEvent : IEvent
    {
        public Guid StateOrderId { get; }
        public OrderExecutionStatus CommandStatus { get; }

        public UpdateOrderExecutionStatusEvent(Guid stateOrderId, OrderExecutionStatus commandStatus)
        {
            StateOrderId = stateOrderId;
            CommandStatus = commandStatus;
        }
    }
}