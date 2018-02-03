using System;
using AkkaExchange.Orders;

namespace AkkaExchange.Client.Events
{
    public class CompleteOrderEvent : Message, IEvent
    {
        public OrderSide Side { get; }
        public decimal Amount { get; }
        public decimal Price { get; }
        
        public CompleteOrderEvent(
            OrderSide side,
            decimal amount,
            decimal price)
        {
            Side = side;
            Amount = amount;
            Price = price;
        }

        public decimal TotalPrice() => Amount * Price;
    }
}