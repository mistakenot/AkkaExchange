﻿using System;

namespace AkkaExchange.Orders.Events
{
    public class AmendOrderEvent : Message, IClientOrderEvent
    {
        public PlacedOrder Order { get; }

        public Guid ClientId => Order.ClientId;

        public AmendOrderEvent(PlacedOrder order)
        {
            Order = order ?? throw new ArgumentNullException(nameof(order));
        }
    }
}