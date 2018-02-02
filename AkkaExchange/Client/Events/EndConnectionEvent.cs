using System;

namespace AkkaExchange.Client.Events
{
    public class EndConnectionEvent : Message, IEvent
    {
        public Guid ClientId { get; }
        public DateTime EndedAt { get; }

        public string ClientName => ClientId.ToString();

        public EndConnectionEvent(Guid clientId)
            : this(clientId, DateTime.UtcNow)
        {
            
        }

        public EndConnectionEvent(Guid clientId, DateTime endedAt)
        {
            ClientId = clientId;
            EndedAt = endedAt;
        }
    }
}