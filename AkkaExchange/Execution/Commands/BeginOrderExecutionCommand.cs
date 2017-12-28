using System;
using AkkaExchange.Orders;

namespace AkkaExchange.Execution.Commands
{
    public class BeginOrderExecutionCommand : ICommand
    {
        public PlacedOrder Order { get; }

        public BeginOrderExecutionCommand(PlacedOrder order)
        {
            Order = order ?? throw new ArgumentNullException(nameof(order));
        }
    }
}