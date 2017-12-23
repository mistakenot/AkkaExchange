using System;
using Akka.Actor;

namespace AkkaExchange
{
    public class AkkaExchange : IDisposable
    {
        public ActorSystem System { get; }

        public AkkaExchange()
        {
            System = ActorSystem.Create("akka-exchange-system");
        }

        public void Dispose()
        {
            System.Dispose();
        }
    }
}