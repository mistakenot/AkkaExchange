using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace AkkaExchange.Web
{
    public class HubSubscriptionCollection
    {
        private readonly IHubContext<AkkaExchangeHub> _context;

        private static readonly ConcurrentDictionary<string, IDisposable> Subscriptions = 
            new ConcurrentDictionary<string, IDisposable>();

        public HubSubscriptionCollection(IHubContext<AkkaExchangeHub> context)
        {
            _context = context;
        }

        public bool TryAdd(string connectionId, IObservable<object> observable)
        {
            var sub = observable.Subscribe(o =>
            {
                _context.Clients.Client(connectionId)?.InvokeAsync("send", o);
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
            IDisposable sub;
            if (Subscriptions.TryRemove(connectionId, out sub))
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