using System;
using AkkaExchange.Orders;

namespace AkkaExchange.Execution.Commands
{
    public class BeginOrderExecutionCommand : Message, ICommand
    {
        public OrderMatch Match { get; }
        
        public BeginOrderExecutionCommand(OrderMatch match)
        {
            Match = match ?? throw new ArgumentNullException(nameof(match));
        }
    }
}