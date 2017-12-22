using System;
using AkkaExchange.State;

namespace AkkaExchange.Commands
{
    public class NewOrderCommand
    {
        public OrderDetails OrderDetails { get; }

        public NewOrderCommand(OrderDetails orderDetails)
        {
            OrderDetails = orderDetails ?? throw new ArgumentNullException(nameof(orderDetails));
        }
    }
}
