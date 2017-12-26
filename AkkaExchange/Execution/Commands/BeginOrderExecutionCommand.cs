using System;
using AkkaExchange.Orders;

namespace AkkaExchange.Execution.Commands
{
    public class BeginOrderExecutionCommand : ICommand
    {
        public Order Order { get; }

        public BeginOrderExecutionCommand(Order order)
        {
            Order = order ?? throw new ArgumentNullException(nameof(order));
        }
    }
}