using System;

namespace AkkaExchange.Orders.Events
{
    public interface IClientOrderEvent : IEvent
    {
        Guid ClientId { get; }
    }
}