using Akka.Actor;
using AkkaExchange.Actors;
using AkkaExchange.Client.Events;

namespace AkkaExchange.Client.Actors
{
    public class ClientActor : BaseActor<ClientState>
    {
        private readonly IActorRef _orderBookActor;

        public ClientActor(
            ICommandHandler<ClientState> handler,
            IActorRef orderBookActor,
            ClientState defaultState) 
            : base(handler, defaultState, defaultState.ClientId.ToString())
        {
            _orderBookActor = orderBookActor;
        }

        protected override void OnPersist(IEvent persistedEvent)
        {
            if (persistedEvent is ExecuteOrderEvent executeOrderCommand)
            {
                // _orderBookActor.Tell(executeOrderCommand.OrderCommand, Self);
            }

            base.OnPersist(persistedEvent);
        }
    }
}