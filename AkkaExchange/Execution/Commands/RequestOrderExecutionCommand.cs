using System;
using AkkaExchange.Orders;

namespace AkkaExchange.Execution.Commands
{
    public class RequestOrderExecutionCommand : ICommand
    {
        public Order Order { get; }

        public RequestOrderExecutionCommand(Order order)
        {
            Order = order ?? throw new ArgumentNullException(nameof(order));
        }
    }
}