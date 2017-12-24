using System;
using AkkaExchange.Client.Events;

namespace AkkaExchange.Client
{
    public class ClientState : IState<ClientState>
    {
        public Guid ClientId { get; }
        public ClientStatus Status { get; }
        public DateTime? StartedAt { get; }
        public DateTime? EndedAt { get; }

        public ClientState(Guid clientId)
            : this(clientId, ClientStatus.Pending, null, null)
        {
            
        }

        private ClientState(Guid clientId, ClientStatus status, DateTime? startedAt, DateTime? endedAt)
        {
            ClientId = clientId;
            Status = status;
            StartedAt = startedAt;
            EndedAt = endedAt;
        }

        public ClientState Update(IEvent evnt)
        {
            if (evnt is StartConnectionEvent startConnectionEvent &&
                Status == ClientStatus.Pending)
            {
                return new ClientState(
                    ClientId,
                    ClientStatus.Connected,
                    startConnectionEvent.StartedAt,
                    null);
            }

            if (evnt is EndConnectionEvent endConnectionEvent &&
                Status == ClientStatus.Connected)
            {
                return new ClientState(
                    ClientId,
                    ClientStatus.Disconnected,
                    StartedAt,
                    endConnectionEvent.EndedAt);
            }

            return this;
        }
    }
}