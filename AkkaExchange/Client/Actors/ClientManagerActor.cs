using Akka.Actor;
using AkkaExchange.Shared.Actors;
using AkkaExchange.Client.Commands;
using AkkaExchange.Client.Events;
using AkkaExchange.Utils;
using System;

namespace AkkaExchange.Client.Actors
{
    public class ClientManagerActor : BaseActor<ClientManagerState>
    {
        private readonly ICommandHandler<ClientState> _clientCommandHandler;
        private readonly IGlobalActorRefs _globalActorRefs;

        public ClientManagerActor(
            ICommandHandler<ClientManagerState> handler,
            ICommandHandler<ClientState> clientCommandHandler,
            IGlobalActorRefs globalActorRefs)
            : base(
                  handler,
                  globalActorRefs,
                  ClientManagerState.Empty,
                  Constants.ClientManagerPersistenceId)
        {
            _clientCommandHandler = clientCommandHandler;
            _globalActorRefs = globalActorRefs;
        }

        protected override void OnCommand(object message)
        {
            if (message is CompleteOrderCommand completeOrderCommand)
            {
                Context.Child(completeOrderCommand.Order.ClientId.ToString()).Tell(message, Sender);
                return;
            }

            base.OnCommand(message);
        }
        protected override void OnPersist(IEvent persistedEvent)
        {
            if (persistedEvent is StartConnectionEvent startConnectionEvent)
            {
                var child = CreateClient(startConnectionEvent);
                Sender.Tell(child, Self);
            }

            if (persistedEvent is EndConnectionEvent endConnectionEvent)
            {
                var child = Context.Child(endConnectionEvent.ClientName);
                Context.Stop(child);
            }

            base.OnPersist(persistedEvent);
        }

        private IActorRef CreateClient(StartConnectionEvent startConnectionEvent)
        {
            var clientState = new ClientState(startConnectionEvent.ClientId);
            var props = Props.Create<ClientActor>(_clientCommandHandler, _globalActorRefs, clientState);
            var child = Context.ActorOf(props, startConnectionEvent.ClientId.ToString());

            child.Tell(
                new StartConnectionCommand(startConnectionEvent.ClientId),
                Sender);

            return child;
        }
    }
}