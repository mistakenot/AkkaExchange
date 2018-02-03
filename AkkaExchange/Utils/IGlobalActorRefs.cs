using Akka.Actor;

namespace AkkaExchange.Utils
{
    public interface IGlobalActorRefs
    {
        IActorRef OrderBook { get; }
        IActorRef ClientManager { get; }
        IActorRef OrderExecutorManager { get; }
        IActorRef ErrorEventSubscriber { get; }
    }

    public class GlobalActorRefs : IGlobalActorRefs
    {
        public IActorRef OrderBook { get; set; }
        public IActorRef ClientManager { get; set; }
        public IActorRef OrderExecutorManager { get; set; }
        public IActorRef ErrorEventSubscriber { get; set; }
    }
}