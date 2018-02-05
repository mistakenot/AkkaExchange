using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace AkkaExchange.Web
{
    public class HubSubscriptionCollection
    {
        private readonly IHubContext<AkkaExchangeHub> _context;
        private readonly ILogger<HubSubscriptionCollection> _logger;
        private static readonly ConcurrentDictionary<string, IDisposable> Subscriptions = 
            new ConcurrentDictionary<string, IDisposable>();

        public HubSubscriptionCollection(
            IHubContext<AkkaExchangeHub> context,
            ILogger<HubSubscriptionCollection> logger)
        {
            _context = context;
            _logger = logger;
        }

        public bool TryAdd(string connectionId, IObservable<object> observable)
        {
            var sub = observable.Subscribe(o =>
            {
                _context.Clients.Client(connectionId)?.InvokeAsync("send", o);
            }, e =>
            {
                _logger.LogError(e, "Error in client event stream.");
            });

            if (Subscriptions.TryAdd(connectionId, sub))
            {
                return true;
            }
            else
            {
                sub.Dispose();
                return false;
            }
        }

        public bool TryDispose(string connectionId)
        {
            if (Subscriptions.TryRemove(connectionId, out var sub))
            {
                sub.Dispose();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}