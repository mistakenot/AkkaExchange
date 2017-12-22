using System;
using System.Collections.Immutable;
using System.Linq;
using AkkaExchange.Orders;
using AkkaExchange.Orders.Events;

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
                var oldOrder = Orders.Single(o => o.OrderId == amendOrderEvent.OrderId);
                var newOrder = new Order(oldOrder.OrderId, amendOrderEvent.OrderDetails);

                return new ExchangeActorState(
                    Orders.RemoveAll(o => o.OrderId == oldOrder.OrderId).Add(newOrder));
            }

            return this;
        }
    }
}