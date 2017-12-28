using Akka.Actor;
using AkkaExchange.Actors;
using AkkaExchange.Client.Events;

namespace AkkaExchange.Client.Actors
{
    public class ClientActor : BaseActor<ClientState>
    {
        private readonly ICanTell _orderBookActor;

        public ClientActor(
            ICommandHandler<ClientState> handler,
            ClientState defaultState) 
            : base(handler, defaultState, defaultState.ClientId.ToString())
        {
            _orderBookActor = Context.ActorSelection("/user/order-book");
        }

        protected override void OnPersist(IEvent persistedEvent)
        {
            if (persistedEvent is ExecuteOrderEvent executeOrderCommand)
            {
                _orderBookActor.Tell(executeOrderCommand.OrderCommand, Self);
            }
            
            base.OnPersist(persistedEvent);
        }
    }
}