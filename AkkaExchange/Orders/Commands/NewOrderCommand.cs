﻿using System;

namespace AkkaExchange.Orders.Commands
{
    public class NewOrderCommand : IOrderCommand
    {
        public Order Order { get; }

        public NewOrderCommand(Order order)
        {
            Order = order ?? throw new ArgumentNullException(nameof(order));
        }
    }
}