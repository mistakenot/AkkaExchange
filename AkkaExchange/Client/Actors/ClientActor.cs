using Akka.Actor;
using AkkaExchange.Shared.Actors;
using AkkaExchange.Client.Events;
using System;
using AkkaExchange.Orders;

namespace AkkaExchange.Client.Actors
{
    public class ClientActor : BaseActor<ClientState>
    {
        private readonly ICanTell _orderBookActor;
        private readonly IObservable<OrderBookState> _orderBookState;

        public ClientActor(
            ICommandHandler<ClientState> handler,
            ClientState defaultState) 
            : base(handler, defaultState, defaultState.ClientId.ToString())
        {
            _orderBookActor = Context.ActorSelection("/user/order-book");
        }

        public override void AroundPreStart()
        {
            base.AroundPreStart();
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