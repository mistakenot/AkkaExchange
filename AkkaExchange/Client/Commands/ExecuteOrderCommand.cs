using System;

namespace AkkaExchange.Client.Commands
{
    public class ExecuteOrderCommand : ICommand
    {
        public Guid ClientId { get; }
        public ICommand OrderCommand { get; }

        public ExecuteOrderCommand(Guid clientId, ICommand orderCommand)
        {
            ClientId = clientId;
            OrderCommand = orderCommand ?? throw new ArgumentNullException(nameof(orderCommand));
        }
    }
}