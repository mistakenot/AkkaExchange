using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace AkkaExchange.Web
{
    public class AkkaExchangeHub : Hub
    {
        public const string ClientKey = "client";
        public const string SubKey = "sub";

        private readonly AkkaExchange _akkaExchange;
        private readonly HubSubscriptionCollection _subscriptions;

        public AkkaExchangeHub(
            AkkaExchange akkaExchange,
            HubSubscriptionCollection subscriptions)
        {
            _akkaExchange = akkaExchange;
            _subscriptions = subscriptions;
        }

        public override async Task OnConnectedAsync()
        {
            var client = await _akkaExchange.NewConnection();

            _subscriptions.TryAdd(Context.ConnectionId, client.Events);
            
            Context.Connection.Metadata.Add(ClientKey, client);

            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var client = Context.Connection.Metadata[ClientKey] as AkkaExchangeClient;
            client?.Dispose();

            _subscriptions.TryDispose(Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }

        public void Bid(decimal price, decimal amount)
        {
            Send(new {price, amount});   
        }
        
        private void Send(object msg) 
            => Clients.Client(Context.Connection.ConnectionId).InvokeAsync("send", msg);
    }
}