using System;
using System.Collections.Immutable;
using AkkaExchange.Client;
using AkkaExchange.Orders;

namespace AkkaExchange.Tests.Client
{
    public abstract class ClientFixture
    {
        public static readonly ClientState ConnectedState = new ClientState(
            Guid.Empty, 
            ClientStatus.Connected,
            DateTime.UtcNow.AddMinutes(-10),
            null,
            ImmutableList<ICommand>.Empty,
            ImmutableList<PlacedOrder>.Empty,
            100m,
            100m);

        public static readonly Order UnplacedBidOrder = new Order(
            Guid.Empty,
            10m,
            1m,
            OrderSide.Bid);

        public static readonly Order UnplacedAskOrder = new Order(
            Guid.Empty,
            10m,
            1m,
            OrderSide.Ask);
    }
}