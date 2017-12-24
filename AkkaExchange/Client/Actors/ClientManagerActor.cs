using Akka.DI.Core;
using AkkaExchange.Actors;
using AkkaExchange.Client.Events;

namespace AkkaExchange.Client.Actors
{
    public class ClientManagerActor : BaseActor<ClientManagerState>
    {
        public ClientManagerActor(
            ICommandHandler<ClientManagerState> handler,  
            string persistenceId) : base(handler, ClientManagerState.Empty, persistenceId)
        {
        }

        protected override void OnPersist(IEvent persistedEvent)
        {
            if (persistedEvent is StartConnectionEvent startConnectionEvent)
            {
                var props = Context.DI().Props<ClientActor>();
                var child = Context.ActorOf(props);
                child.Tell(startConnectionEvent, Self);
                Sender.Tell(child, Self);
            }

            if (persistedEvent is EndConnectionEvent endConnectionEvent)
            {
                var child = Context.Child(endConnectionEvent.ClientName);
                child.Tell(persistedEvent, Sender);
                Context.Stop(child);
            }

            base.OnPersist(persistedEvent);
        }
    }
}