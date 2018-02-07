using System;
using AkkaExchange.Orders;

namespace AkkaExchange.Client.Commands
{
    public class CompleteOrderCommand : Message, ICommand
    {
        public PlacedOrder Order { get; }
        
        public CompleteOrderCommand(PlacedOrder order)
        {
            Order = order;
        }
    }
}