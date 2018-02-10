using System;
using AkkaExchange.Orders;

namespace AkkaExchange.Orders.Events
{
    public class CompleteOrderEvent : Message, IEvent
    {
        public PlacedOrder Bid { get; }
        public PlacedOrder Ask { get; }

        public CompleteOrderEvent(
            PlacedOrder bid, 
            PlacedOrder ask)
        {
            Bid = bid ?? throw new ArgumentNullException(nameof(bid));
            Ask = ask ?? throw new ArgumentNullException(nameof(ask));
        }
    }
}