using System;
using System.IO;
using Akka.Configuration;
using AkkaExchange.Orders;
using AkkaExchange.Utils;
using Autofac;
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

            using (var exchange = new AkkaExchange(builder, config))
            {
                var client = exchange.NewConnection().Result;
                var clientSubscription = client.Events.Subscribe(e =>
                {
                    Console.WriteLine($"Client has received event: {e}");
                });

                var managerSubscription = exchange.Queries.ClientManagerState.Subscribe(s => Console.WriteLine(JsonConvert.SerializeObject(s)));
                var orderBookSubscription = exchange.Queries.OrderBookState.Subscribe(s => Console.WriteLine(JsonConvert.SerializeObject(s)));

                client.NewOrder(1m, 1m, OrderSide.Ask);
                client.NewOrder(2m, 2m, OrderSide.Bid);

                Console.ReadLine();

                clientSubscription.Dispose();
            }

        }
    }
}
