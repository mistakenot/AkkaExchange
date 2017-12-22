using System;

namespace AkkaExchange.Orders
{
    public class OrderDetails
    {
        public decimal Amount { get; }
        public decimal Price { get; }
        public OrderStateSide Side { get; }

        public OrderDetails(decimal amount, decimal price, OrderStateSide side)
        {
            if (amount < 0m) throw new ArgumentException("Amount must be > 0.");
            if (price < 0m) throw new ArgumentException("Price must be > 0.");

            Amount = amount;
            Price = price;
            Side = side;
        }
    }

    public enum OrderStateSide
    {
        Ask, Bid
    }
}
