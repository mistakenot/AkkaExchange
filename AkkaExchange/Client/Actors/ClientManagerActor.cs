﻿using Akka.Actor;
using AkkaExchange.Actors;
using AkkaExchange.Client.Commands;
using AkkaExchange.Client.Events;
using AkkaExchange.Utils;

namespace AkkaExchange.Client.Actors
{
    public class ClientManagerActor : BaseActor<ClientManagerState>
    {
        private readonly ICommandHandler<ClientState> _clientCommandHandler;

        public ClientManagerActor(
            ICommandHandler<ClientManagerState> handler,
            ICommandHandler<ClientState> clientCommandHandler) 
            : base(
                  handler,
                  ClientManagerState.Empty, 
                  Constants.ClientManagerPersistenceId)
        {
            _clientCommandHandler = clientCommandHandler;
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
                StopClient(endConnectionEvent);
            }

            base.OnPersist(persistedEvent);
        }

        private void StopClient(EndConnectionEvent endConnectionEvent)
        {
            var child = Context.Child(endConnectionEvent.ClientName);
            child.Tell(new EndConnectionCommand(endConnectionEvent.ClientId), Sender);
            Context.Stop(child);
        }

        private IActorRef CreateClient(StartConnectionEvent startConnectionEvent)
        {
            var clientState = new ClientState(startConnectionEvent.ClientId);
            var props = Props.Create<ClientActor>(_clientCommandHandler, null, clientState);
            var child = Context.ActorOf(props, startConnectionEvent.ClientId.ToString());

            child.Tell(
                new StartConnectionCommand(startConnectionEvent.ClientId),
                Sender);

            return child;
        }

        protected override void OnRecover(object persistedEvent)
        {
            if (persistedEvent is StartConnectionEvent startConnectionEvent)
            {
                CreateClient(startConnectionEvent);
            }

            if (persistedEvent is EndConnectionEvent endConnectionEvent)
            {
                StopClient(endConnectionEvent);
            }

            base.OnRecover(persistedEvent);
        }
    }
}