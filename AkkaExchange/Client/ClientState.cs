using System;
using System.Collections.Immutable;
using AkkaExchange.Client.Events;
using AkkaExchange.Orders;

namespace AkkaExchange.Client
{
    public class ClientState : Message, IState<ClientState>
    {
        public Guid ClientId { get; }
        public ClientStatus Status { get; }
        public DateTime? StartedAt { get; }
        public DateTime? EndedAt { get; }
        public decimal Balance { get; }
        public IImmutableList<ICommand> OrderCommandHistory { get; }

        public static readonly ClientState Empty = new ClientState(Guid.Empty);

        public ClientState(Guid clientId)
            : this(clientId, ClientStatus.Pending, null, null, ImmutableList<ICommand>.Empty, 100m)
        {
            
        }

        private ClientState(
            Guid clientId, 
            ClientStatus status, 
            DateTime? startedAt, 
            DateTime? endedAt, 
            IImmutableList<ICommand> orderCommandHistory, 
            decimal balance)
        {
            ClientId = clientId;
            Status = status;
            StartedAt = startedAt;
            EndedAt = endedAt;
            OrderCommandHistory = orderCommandHistory;
            Balance = balance;
        }

        public ClientState Update(IEvent evnt)
        {
            if (evnt is StartConnectionEvent startConnectionEvent &&
                Status == ClientStatus.Pending)
            {
                return new ClientState(
                    startConnectionEvent.ClientId,
                    ClientStatus.Connected,
                    startConnectionEvent.StartedAt,
                    null,
                    OrderCommandHistory,
                    Balance);
            }

            if (evnt is EndConnectionEvent endConnectionEvent &&
                Status == ClientStatus.Connected)
            {
                return new ClientState(
                    ClientId,
                    ClientStatus.Disconnected,
                    StartedAt,
                    endConnectionEvent.EndedAt,
                    OrderCommandHistory,
                    Balance);
            }
            
            if (evnt is ExecuteOrderEvent executeOrderEvent)
            {
                return new ClientState(
                    ClientId,
                    Status,
                    StartedAt,
                    EndedAt,
                    OrderCommandHistory.Add(executeOrderEvent.OrderCommand),
                    Balance);
            }

            if (evnt is CompleteOrderEvent completeOrderEvent &&
                Status == ClientStatus.Connected)
            {
                return new ClientState(
                    ClientId,
                    Status,
                    StartedAt,
                    EndedAt,
                    OrderCommandHistory,
                    completeOrderEvent.Side == OrderSide.Bid ? 
                        Balance - completeOrderEvent.Amount : 
                        Balance + completeOrderEvent.Amount);
            }
            return this;
        }
    }
}