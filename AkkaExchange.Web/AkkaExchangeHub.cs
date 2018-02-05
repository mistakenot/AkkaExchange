using System;
using System.Threading.Tasks;
using AkkaExchange.Orders;
using AkkaExchange.Client.Extensions;
using Microsoft.AspNetCore.SignalR;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;

namespace AkkaExchange.Web
{
    public class AkkaExchangeHub : Hub
    {
        private readonly HubSubscriptionCollection _subscriptions;
        private readonly HubClientCollection _clients;
        private readonly AkkaExchange _akkaExchange;
        private readonly ILogger<AkkaExchangeHub> _logger;

        public AkkaExchangeHub( 
            HubSubscriptionCollection subscriptions,
            HubClientCollection clients,
            AkkaExchange akkaExchange,
            ILogger<AkkaExchangeHub> logger)
        {
            _subscriptions = subscriptions;
            _clients = clients;
            _akkaExchange = akkaExchange;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"OnConnect ConnectionId: {Context.ConnectionId}");

            var client = await _clients.GetClient(Context.ConnectionId);
            var observable = client.Events
                .Merge(
                    client.State.Cast<object>())
                .Merge(
                    client.Errors.Cast<object>())
                .Merge(
                    _akkaExchange.Queries.PlacedOrderVolumePerTenSeconds.Cast<object>())
                .Merge(
                    _akkaExchange.Queries.ClientManagerState.NumberOfConnectedClientsQuery().Cast<object>());
            
            _subscriptions.TryAdd(Context.ConnectionId, observable);
            
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation($"OnDisconnect ConnectionId: {Context.ConnectionId}");
            
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