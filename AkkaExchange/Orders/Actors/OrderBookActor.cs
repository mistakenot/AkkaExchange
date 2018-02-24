using System;
using Akka.Actor;
using AkkaExchange.Execution.Commands;
using AkkaExchange.Orders.Commands;
using AkkaExchange.Orders.Events;
using AkkaExchange.Shared.Actors;
using AkkaExchange.Utils;

namespace AkkaExchange.Orders.Actors
{
    /// <summary>
    /// Maintains the current open order book.
    /// </summary>
    public class OrderBookActor : BaseActor<OrderBookState>
    {
        private readonly IActorRef _orderExecutorManager;
        private readonly IActorRef _clientManager;

        public OrderBookActor(
            ICommandHandler<OrderBookState> commandHandler,
            IGlobalActorRefs globalActorRefs)
            : base(
                commandHandler,
                globalActorRefs,
                OrderBookState.Empty,
                Constants.OrderBookPersistenceId)
        {
            _orderExecutorManager = globalActorRefs?.OrderExecutorManager ?? throw new ArgumentNullException(nameof(globalActorRefs));
            _clientManager = globalActorRefs?.ClientManager ?? throw new ArgumentNullException(nameof(globalActorRefs));
        }

        protected override void OnCommand(object message)
        {
            if (message is MatchOrdersCommand && State.OpenOrders.Count == 0)
            {
                // Hack to stop us filling up the db with null events.
            }
            else
            {
                OnCommand(message);
            }
        }
        protected override void OnPersist(IEvent persistedEvent)
        {
            if (persistedEvent is MatchedOrdersEvent matchedOrdersEvent)
            {
                foreach (var matchedOrdersMatch in matchedOrdersEvent.MatchedOrders.Matches)
                {
                    _orderExecutorManager.Tell(
                        new BeginOrderExecutionCommand(
                            matchedOrdersMatch));
                }
            }

            if (persistedEvent is CompleteOrderEvent completeOrderEvent)
            {
                var askEvent = new Client.Commands.CompleteOrderCommand(completeOrderEvent.Ask);
                var bidEvent = new Client.Commands.CompleteOrderCommand(completeOrderEvent.Bid);
                
                _clientManager.Tell(askEvent, Self);
                _clientManager.Tell(bidEvent, Self);
            }

            base.OnPersist(persistedEvent);
        }
    }
}