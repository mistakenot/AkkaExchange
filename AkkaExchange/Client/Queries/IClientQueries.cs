using System;
using AkkaExchange.Utils;

namespace AkkaExchange.Client.Queries
{
    public interface IClientQueries
    {
         IObservable<ClientManagerState> ClientManagerState { get; }

         IObservable<IEvent> Events(string persistenceId);
         IObservable<ClientState> State(string persistenceId);
         IObservable<HandlerResult> Errors(string persistenceId);
    }
}