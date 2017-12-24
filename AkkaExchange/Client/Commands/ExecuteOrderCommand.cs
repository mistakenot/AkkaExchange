using System;

namespace AkkaExchange.Client.Commands
{
    public class ExecuteOrderCommand : ICommand
    {
        public Guid ClientId { get; }
        public dynamic OrderCommand { get; }

        public ExecuteOrderCommand(Guid clientId, dynamic orderCommand)
        {
            ClientId = clientId;
            OrderCommand = orderCommand ?? throw new ArgumentNullException(nameof(orderCommand));
        }
    }
}