using System;
using System.Collections.Immutable;

namespace AkkaExchange.Orders.Commands
{
    public class BeginMatchOrdersCommand : ICommand
    {
        public IImmutableList<Order> Orders { get; }

        public BeginMatchOrdersCommand(IImmutableList<Order> orders)
        {
            Orders = orders ?? throw new ArgumentNullException(nameof(orders));
        }
    }
}
