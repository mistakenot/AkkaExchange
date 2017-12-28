using AkkaExchange.Orders;

namespace AkkaExchange.Execution.Events
{
    public class BeginOrderExecutionEvent : IEvent
    {
        public PlacedOrder Order { get; }

        public BeginOrderExecutionEvent(PlacedOrder order)
        {
            Order = order;
        }
    }
}