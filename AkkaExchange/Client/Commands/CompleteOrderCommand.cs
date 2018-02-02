using System;
using AkkaExchange.Orders;

namespace AkkaExchange.Client.Commands
{
    public class CompleteOrderCommand : Message, ICommand
    {
        public Order Order { get; }
        
        public CompleteOrderCommand(
            Order order)
        {
            Order = order;
        }
    }
}