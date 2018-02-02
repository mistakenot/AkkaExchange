using System;
using System.Threading.Tasks;
using AkkaExchange.Orders;
using Microsoft.AspNetCore.SignalR;
using System.Reactive.Linq;

namespace AkkaExchange.Web
{
    public class AkkaExchangeHub : Hub
    {
        private readonly HubSubscriptionCollection _subscriptions;
        private readonly HubClientCollection _clients;
        private readonly AkkaExchange _akkaExchange;

        public AkkaExchangeHub( 
            HubSubscriptionCollection subscriptions,
            HubClientCollection clients,
            AkkaExchange akkaExchange)
        {
            _subscriptions = subscriptions;
            _clients = clients;
            _akkaExchange = akkaExchange;
        }

        public override async Task OnConnectedAsync()
        {
            var client = await _clients.GetClient(Context.ConnectionId);
            var subscription = client.Events
                .Merge(client.State.Cast<object>())
                .Merge(_akkaExchange.Queries.ClientManagerState.Cast<object>())
                .Merge(_akkaExchange.Queries.PlacedOrderVolumePerTenSeconds.Cast<object>())
                .Merge(_akkaExchange.Queries.OrderBookState.Cast<object>());
            
            _subscriptions.TryAdd(Context.ConnectionId, subscription);
            
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _clients.DisposeClient(Context.ConnectionId);
            _subscriptions.TryDispose(Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task Bid(decimal price, decimal amount)
        {
            var client = await _clients.GetClient(Context.ConnectionId);
            client.NewOrder(price, amount, OrderSide.Bid);
        }

        public async Task Ask(decimal price, decimal amount)
        {
            var client = await _clients.GetClient(Context.ConnectionId);
            client.NewOrder(price, amount, OrderSide.Ask);
        }
    }
}