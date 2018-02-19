using System;
using AkkaExchange.Utils;

namespace AkkaExchange.Client.Queries
{
    public interface IClientQueries
    {
         IObservable<ClientManagerState> ClientManagerState { get; }

         IObservable<IEvent> Events(Guid clientId);
         IObservable<ClientState> State(Guid clientId);
         IObservable<HandlerResult> Errors(Guid clientId);
    }
}