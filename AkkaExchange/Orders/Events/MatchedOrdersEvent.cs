﻿using System;

namespace AkkaExchange.Orders.Events
{
    public class MatchedOrdersEvent : Message, IEvent
    {
        public OrderMatchResult MatchedOrders { get; }

        public MatchedOrdersEvent(OrderMatchResult matchedOrders)
        {
            MatchedOrders = matchedOrders ?? throw new ArgumentNullException(nameof(matchedOrders));
        }
    }
}
