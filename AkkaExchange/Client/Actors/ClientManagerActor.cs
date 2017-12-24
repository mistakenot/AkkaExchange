using Akka.DI.Core;
using AkkaExchange.Actors;
using AkkaExchange.Client.Commands;
using AkkaExchange.Client.Events;
using AkkaExchange.Utils;

namespace AkkaExchange.Client.Actors
{
    public class ClientManagerActor : BaseActor<ClientManagerState>
    {
        public ClientManagerActor(
            ICommandHandler<ClientManagerState> handler) 
            : base(
                  handler,
                  ClientManagerState.Empty, 
                  Constants.ClientManagerPersistenceId)
        {
        }

        protected override void OnPersist(IEvent persistedEvent)
        {
            if (persistedEvent is StartConnectionEvent startConnectionEvent)
            {
                var props = Context.DI().Props<ClientActor>();
                var child = Context.ActorOf(props, startConnectionEvent.ClientId.ToString());
                child.Tell(startConnectionEvent, Self);
                Sender.Tell(child, Self);
            }

            if (persistedEvent is EndConnectionEvent endConnectionEvent)
            {
                var child = Context.Child(endConnectionEvent.ClientName);
                child.Tell(new EndConnectionCommand(endConnectionEvent.ClientId), Sender);
                Context.Stop(child);
            }

            base.OnPersist(persistedEvent);
        }
    }
}