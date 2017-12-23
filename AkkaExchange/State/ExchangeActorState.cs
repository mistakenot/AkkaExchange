using System;
using System.Collections.Immutable;
using System.Linq;
using AkkaExchange.Orders;
using AkkaExchange.Orders.Events;
using AkkaExchange.Orders.Extensions;

namespace AkkaExchange.State
{
    public class ExchangeActorState : IState<ExchangeActorState>
    {
        public IImmutableList<Order> Orders { get; }

        public ExchangeActorState()
            : this(ImmutableList<Order>.Empty)
        { 
        }

        public ExchangeActorState(IImmutableList<Order> orders)
        {
            Orders = orders ?? throw new ArgumentNullException(nameof(orders));
        }

        public ExchangeActorState Update(IEvent evnt)
        {
            if (evnt is NewOrderEvent newOrderEvent)
            {
                return new ExchangeActorState(
                    Orders.Add(newOrderEvent.Order));
            }
            else if (evnt is AmendOrderEvent amendOrderEvent)
            {
                return new ExchangeActorState(
                    Orders
                        .RemoveAll(o => o.OrderId == amendOrderEvent.Order.OrderId)
                        .Add(amendOrderEvent.Order));
            }

            return this;
        }
    }
}