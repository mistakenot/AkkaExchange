using System;
using System.Collections.Generic;

namespace AkkaExchange.Orders
{
    public class OrderBook
    {
        public IEnumerable<Order> Orders { get; }

        public OrderBook(IEnumerable<Order> orders)
        {
            Orders = orders ?? throw new ArgumentNullException(nameof(orders));
        }
    }
}
