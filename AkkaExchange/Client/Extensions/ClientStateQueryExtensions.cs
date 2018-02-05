using System;
using System.Reactive.Linq;
using AkkaExchange.Client.Queries;

namespace AkkaExchange.Client.Extensions
{
    public static class ClientStateQueryExtensions
    {
        public static IObservable<NumberOfConnectedClients> NumberOfConnectedClientsQuery(this IObservable<ClientManagerState> stateQuery) =>
            stateQuery.Select(s => new NumberOfConnectedClients(s.ClientIds.Count));
    }
}