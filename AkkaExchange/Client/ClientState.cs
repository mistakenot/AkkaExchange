using System;
using System.Collections.Immutable;
using AkkaExchange.Client.Events;

namespace AkkaExchange.Client
{
    public class ClientState : IState<ClientState>
    {
        public Guid ClientId { get; }
        public ClientStatus Status { get; }
        public DateTime? StartedAt { get; }
        public DateTime? EndedAt { get; }
        public IImmutableList<ICommand> OrderCommandHistory { get; }

        public ClientState(Guid clientId)
            : this(clientId, ClientStatus.Pending, null, null, ImmutableList<ICommand>.Empty)
        {
            
        }

        private ClientState(Guid clientId, ClientStatus status, DateTime? startedAt, DateTime? endedAt, IImmutableList<ICommand> orderCommandHistory)
        {
            ClientId = clientId;
            Status = status;
            StartedAt = startedAt;
            EndedAt = endedAt;
            OrderCommandHistory = orderCommandHistory;
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
                    null,
                    OrderCommandHistory);
            }

            if (evnt is EndConnectionEvent endConnectionEvent &&
                Status == ClientStatus.Connected)
            {
                return new ClientState(
                    ClientId,
                    ClientStatus.Disconnected,
                    StartedAt,
                    endConnectionEvent.EndedAt,
                    OrderCommandHistory);
            }

            if (evnt is ExecuteOrderEvent executeOrderEvent)
            {
                return new ClientState(
                    ClientId,
                    Status,
                    StartedAt,
                    EndedAt,
                    OrderCommandHistory.Add(executeOrderEvent.OrderCommand));
            }

            return this;
        }
    }
}