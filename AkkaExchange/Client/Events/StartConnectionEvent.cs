using System;

namespace AkkaExchange.Client.Events
{
    public class StartConnectionEvent : IEvent
    {
        public Guid ClientId { get; }
        public DateTime StartedAt { get; }

        public StartConnectionEvent()
            : this(Guid.NewGuid(), DateTime.UtcNow)
        {
            
        }

        public StartConnectionEvent(Guid clientId, DateTime startedAt)
        {
            ClientId = clientId;
            StartedAt = startedAt;
        }
    }
}