using System;

namespace AkkaExchange.Client.Events
{
    public class ExecuteOrderEvent : Message, IEvent
    {
        public Guid ClientId { get; }
        public ICommand OrderCommand { get; }

        public ExecuteOrderEvent(Guid clientId, ICommand orderCommand)
        {
            ClientId = clientId;
            OrderCommand = orderCommand ?? throw new ArgumentNullException(nameof(orderCommand));
        }
    }
}