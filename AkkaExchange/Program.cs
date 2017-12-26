using System;
using System.IO;
using Akka.Configuration;
using AkkaExchange.Orders;
using AkkaExchange.Utils;

namespace AkkaExchange
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Exchange...");

            var container = new Autofac.ContainerBuilder()
                .AddAkkaExchangeDependencies()
                .Build();

            var configString = File.ReadAllText("config.txt");
            var config = ConfigurationFactory.ParseString(configString);

            using (var exchange = new AkkaExchange(container, config))
            {
                var client = exchange.NewConnection().Result;
                var clientSubscription = client.Events.Subscribe(e =>
                {
                    Console.WriteLine($"Client has received event: {e}");
                });

                client.NewOrder(1m, 1m, OrderSide.Ask);
                client.NewOrder(2m, 2m, OrderSide.Bid);

                Console.ReadLine();

                clientSubscription.Dispose();
            }

        }
    }
}
