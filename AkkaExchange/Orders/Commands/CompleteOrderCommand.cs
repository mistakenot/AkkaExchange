using System;

namespace AkkaExchange.Orders.Commands
{
    public class CompleteOrdersCommand : Message, ICommand
    {
        public OrderMatch Match { get; }

        public CompleteOrdersCommand(OrderMatch orderMatch)
        {
            Match = orderMatch ?? throw new ArgumentNullException(nameof(orderMatch));
        }
    }
}