using System;
using System.IO;
using System.Linq;
using Akka.Actor;
using Akka.Configuration;
using AkkaExchange.Client.Extensions;
using AkkaExchange.Orders;
using AkkaExchange.Orders.Events;
using AkkaExchange.Shared.Extensions;
using AkkaExchange.Utils;
using Autofac;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AkkaExchange
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Exchange...");

            var builder = new ContainerBuilder().AddAkkaExchangeDependencies();

            var configString = File.ReadAllText("config.txt");
            var config = ConfigurationFactory.ParseString(configString);
            var logger = new LoggerFactory().CreateLogger<AkkaExchange>();

            using (var exchange = new AkkaExchange(builder, config, logger))
            {
                var client = exchange.NewConnection().Result;
                var clientSubscription = client.Events.Subscribe(e =>
                {
                    Console.WriteLine($"Client has received event: {e}");
                });
                
                var orderBookSubscription = exchange.Queries.OrderBookState.Subscribe(s => 
                    Console.WriteLine($"Open: {s.OpenOrders.Count}, Executing: {s.ExecutingOrders.Count}, Complete: {s.CompleteOrders.Count}."));
                var placedOrderVolumeSubscription = exchange.Queries.PlacedOrderVolumePerTenSeconds.Subscribe(s => 
                    Console.WriteLine($"Volume in last minute: {s.Volume}"));
                var errorSubscription = exchange.Queries.HandlerErrorEvents.Subscribe(e => 
                    Console.WriteLine($"Error: {e.ToString()}"));
                var orderCompleteSubscription = exchange.Queries.OrderEventStream.Match<CompleteOrderEvent, IEvent>().CompletedOrders().Subscribe(e => 
                    Console.WriteLine($"CompleteOrders: {e.Orders.Count()}"));
                    
                client.NewOrder(1m, 1m, OrderSide.Ask);
                client.NewOrder(1m, 1m, OrderSide.Bid);

                Console.ReadLine();

                clientSubscription.Dispose();
            }

        }
    }
}
