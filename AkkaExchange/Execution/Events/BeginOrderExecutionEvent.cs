using AkkaExchange.Orders;

namespace AkkaExchange.Execution.Events
{
    public class BeginOrderExecutionEvent : IEvent
    {
        public Order Order { get; }

        public BeginOrderExecutionEvent(Order order)
        {
            Order = order;
        }
    }
}