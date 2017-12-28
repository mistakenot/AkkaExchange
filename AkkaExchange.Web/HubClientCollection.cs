using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace AkkaExchange.Web
{
    public class HubClientCollection
    {
        private readonly AkkaExchange _akkaExchange;

        private static readonly ConcurrentDictionary<string, Task<AkkaExchangeClient>> Clients 
            = new ConcurrentDictionary<string, Task<AkkaExchangeClient>>();

        public HubClientCollection(AkkaExchange akkaExchange)
        {
            _akkaExchange = akkaExchange;
        }

        public Task<AkkaExchangeClient> GetClient(string connectionId)
        {
            return Clients.GetOrAdd(connectionId, id => _akkaExchange.NewConnection());
        }

        public async Task DisposeClient(string clientId)
        {
            if (Clients.TryRemove(clientId, out var valueFactory))
            {
                var value = await valueFactory;
                value.Dispose();
            }
        }
    }
}