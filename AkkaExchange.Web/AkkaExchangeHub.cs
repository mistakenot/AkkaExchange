using System;
using System.Threading.Tasks;
using AkkaExchange.Orders;
using Microsoft.AspNetCore.SignalR;

namespace AkkaExchange.Web
{
    public class AkkaExchangeHub : Hub
    {
        private readonly HubSubscriptionCollection _subscriptions;
        private readonly HubClientCollection _clients;

        public AkkaExchangeHub(
            HubSubscriptionCollection subscriptions,
            HubClientCollection clients)
        {
            _subscriptions = subscriptions;
            _clients = clients;
        }

        public override async Task OnConnectedAsync()
        {
            var client = await _clients.GetClient(Context.ConnectionId);
            _subscriptions.TryAdd(Context.ConnectionId, client.Events);
            
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