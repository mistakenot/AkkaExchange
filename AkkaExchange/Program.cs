using System;
using Akka.DI.AutoFac;
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
            
            using (var exchange = new AkkaExchange())
            {
                var resolver = new AutoFacDependencyResolver(container, exchange.System);
                Console.ReadLine();
            }
        }
    }
}
