﻿using System;

namespace AkkaExchange.Orders.Events
{
    public class AmendOrderEvent : IEvent
    {
        public Guid OrderId { get; }
        public OrderDetails OrderDetails { get; }

        public AmendOrderEvent(
            Guid orderId,
            OrderDetails order)
        {
            OrderId = orderId;
            OrderDetails = order ?? throw new ArgumentNullException(nameof(order));
        }
    }
}