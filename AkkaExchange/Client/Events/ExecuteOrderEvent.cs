using System;

namespace AkkaExchange.Client.Events
{
    public class ExecuteOrderEvent
    {
        public Guid ClientId { get; }
        public dynamic OrderEvent { get; }

        public ExecuteOrderEvent(Guid clientId, dynamic orderEvent)
        {
            ClientId = clientId;
            OrderEvent = orderEvent ?? throw new ArgumentNullException(nameof(orderEvent));
        }
    }
}