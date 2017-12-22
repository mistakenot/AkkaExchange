using AkkaExchange.Orders;
using System.Collections.Immutable;

namespace AkkaExchange.Matching.Events
{
    public class BeginMatchOrdersEvent : IEvent
    {
        public IImmutableList<Order> StateOrders { get; }

        public BeginMatchOrdersEvent(IImmutableList<Order> stateOrders)
        {
            StateOrders = stateOrders;
        }
    }
}