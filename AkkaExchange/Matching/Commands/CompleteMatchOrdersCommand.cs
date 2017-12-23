using System;

namespace AkkaExchange.Matching.Commands
{
    public class CompleteMatchOrdersCommand : ICommand
    {
        public OrderMatchResult MatchedOrders { get; }

        public CompleteMatchOrdersCommand(OrderMatchResult matchedOrders)
        {
            MatchedOrders = matchedOrders ?? throw new ArgumentNullException(nameof(matchedOrders));
        }
    }

    public class CompleteMatchOrdersCommandHandler : BaseCommandHandler<CompleteMatchOrdersCommand, float>
    {
        
    }
}
