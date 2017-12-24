using System;
using Akka.Actor;

namespace AkkaExchange
{
    public class AkkaExchangeClient
    {
        private readonly IActorRef _actor;

        internal AkkaExchangeClient(IActorRef actor)
        {
            _actor = actor ?? throw new ArgumentNullException(nameof(actor));
        }
    }
}