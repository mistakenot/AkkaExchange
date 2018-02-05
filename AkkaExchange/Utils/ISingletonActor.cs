using Akka.Actor;

namespace AkkaExchange.Utils
{
    public interface ISingletonActor<TActor>
    {
        IActorRef Ref { get; }
    }

    public class SingletonActor<TActor> : ISingletonActor<TActor>
    {
        public IActorRef Ref { get; }

        public SingletonActor(IActorRef actorRef) => 
            Ref = actorRef ?? throw new System.ArgumentNullException(nameof(actorRef));
    }
}