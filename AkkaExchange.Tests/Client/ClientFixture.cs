using System;
using System.Collections.Immutable;
using AkkaExchange.Client;
using AkkaExchange.Orders;

namespace AkkaExchange.Tests.Client
{
    public abstract class ClientFixture
    {
        protected ClientState ConnectedState = new ClientState(
            Guid.Empty, 
            ClientStatus.Connected,
            DateTime.UtcNow.AddMinutes(-10),
            null,
            ImmutableList<ICommand>.Empty,
            100m,
            100m);

        protected Order UnplacedBidOrder = new Order(
            Guid.Empty,
            10m,
            1m,
            OrderSide.Bid);
    }
}