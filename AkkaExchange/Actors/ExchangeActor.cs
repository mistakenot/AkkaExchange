using Akka.Persistence;
using AkkaExchange.Events;
using AkkaExchange.Handlers;
using AkkaExchange.State;

namespace AkkaExchange.Actors
{
    public class ExchangeActor : BaseActor<ExchangeActorState, ExchangeCommandHandler>
    {
        public ExchangeActor()
            : base(
                  new ExchangeCommandHandler(), 
                  new ExchangeActorState(), 
                  "exchange-actor")
        {
        }
    }
}
