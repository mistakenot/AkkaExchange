using System.Collections.Immutable;
using AkkaExchange.State;

namespace AkkaExchange.Events
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