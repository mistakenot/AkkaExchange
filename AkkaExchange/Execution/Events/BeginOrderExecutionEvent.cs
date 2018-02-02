using AkkaExchange.Orders;

namespace AkkaExchange.Execution.Events
{
    public class BeginOrderExecutionEvent : Message, IEvent
    {
        public PlacedOrder Order { get; }

        public BeginOrderExecutionEvent(PlacedOrder order)
        {
            Order = order;
        }
    }
}