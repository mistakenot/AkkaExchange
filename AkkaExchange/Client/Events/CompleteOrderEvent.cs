using System;
using AkkaExchange.Orders;

namespace AkkaExchange.Client.Events
{
    public class CompleteOrderEvent : Message, IEvent
    {
        public OrderSide Side { get; }
        public decimal Amount { get; }
        
        public CompleteOrderEvent(
            OrderSide side,
            decimal amount)
        {
            Side = side;
            Amount = amount;
        }
    }
}