using System;
using System.Collections.Generic;
using System.Linq;
using AkkaExchange.Orders.Events;

namespace AkkaExchange.Orders.Queries
{
    public class PlacedOrderVolume
    {
        public DateTime From { get; }
        public DateTime Until { get; }
        public decimal Volume { get; }

        public PlacedOrderVolume(DateTime @from, DateTime until, decimal volume)
        {
            From = @from;
            Until = until;
            Volume = volume;
        }

        public static PlacedOrderVolume FromEnumerable(IEnumerable<NewOrderEvent> events)
        {
            var es = events.ToList();
            var from = es.Min(e => e.Order.PlacedAt);
            var to = es.Max(e => e.Order.PlacedAt);
            var volume = es.Sum(e => e.Order.Amount);

            return new PlacedOrderVolume(
                from,
                to,
                volume);
        }
    }
}