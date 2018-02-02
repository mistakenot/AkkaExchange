using System;

namespace AkkaExchange.Orders.Events
{
    public class NewOrderEvent : Message, IClientOrderEvent
    {
        public PlacedOrder Order { get; }

        public Guid ClientId => Order.ClientId;

        public NewOrderEvent(PlacedOrder order)
        {
            Order = order ?? throw new ArgumentNullException(nameof(order));
        }
    }
}
