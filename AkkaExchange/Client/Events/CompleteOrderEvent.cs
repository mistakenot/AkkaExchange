using System;
using AkkaExchange.Orders;

namespace AkkaExchange.Client.Events
{
    public class CompleteOrderEvent : Message, IEvent
    {
        public PlacedOrder Order { get; }
        
        public CompleteOrderEvent(PlacedOrder order)
        {
            Order = order;
        }
    }
}