using System;

namespace AkkaExchange.Orders.Commands
{
    public class NewOrderCommand : IOrderCommand
    {
        public OrderDetails OrderDetails { get; }

        public NewOrderCommand(OrderDetails orderDetails)
        {
            OrderDetails = orderDetails ?? throw new ArgumentNullException(nameof(orderDetails));
        }
    }
}
