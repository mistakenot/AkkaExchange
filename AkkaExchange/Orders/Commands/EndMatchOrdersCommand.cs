using System;

namespace AkkaExchange.Orders.Commands
{
    public class EndMatchOrdersCommand : ICommand
    {
        public OrderMatchResult MatchedOrders { get; }

        public EndMatchOrdersCommand(OrderMatchResult matchedOrders)
        {
            MatchedOrders = matchedOrders ?? throw new ArgumentNullException(nameof(matchedOrders));
        }
    }
}
