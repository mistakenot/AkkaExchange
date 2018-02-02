using System;
using System.Collections.Immutable;
using AkkaExchange.Client.Events;

namespace AkkaExchange.Client
{
    public class ClientManagerState : Message, IState<ClientManagerState>
    {
        public IImmutableList<Guid> ClientIds { get; }

        public static ClientManagerState Empty = 
            new ClientManagerState(
                ImmutableArray<Guid>.Empty);

        public ClientManagerState(IImmutableList<Guid> clientIds)
        {
            ClientIds = clientIds ?? throw new ArgumentNullException(nameof(clientIds));
        }

        public ClientManagerState Update(IEvent evnt)
        {
            if (evnt is StartConnectionEvent startConnectionEvent)
            {
                return new ClientManagerState(
                    ClientIds.Add(startConnectionEvent.ClientId));
            }

            if (evnt is EndConnectionEvent endConnectionEvent)
            {
                return new ClientManagerState(
                    ClientIds.Remove(endConnectionEvent.ClientId));
            }

            return this;
        }
    }
}