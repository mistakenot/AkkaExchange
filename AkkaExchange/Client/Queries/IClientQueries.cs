using System;

namespace AkkaExchange.Client.Queries
{
    public interface IClientQueries
    {
         IObservable<ClientManagerState> ClientManagerState { get ;}
    }
}