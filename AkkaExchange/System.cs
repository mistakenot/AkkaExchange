using System;
using Akka.Actor;

namespace AkkaExchange
{
    public class System : IDisposable
    {
        private readonly ActorSystem _system;

        public System()
        {
            _system = ActorSystem.Create("akka-exchange-system");
        }

        public void Dispose()
        {
            _system.Dispose();
        }
    }
}