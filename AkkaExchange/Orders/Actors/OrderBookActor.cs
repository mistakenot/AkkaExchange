﻿using System;
using Akka.Actor;
using AkkaExchange.Execution.Commands;
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

        public OrderBookActor(
            ICommandHandler<OrderBookState> commandHandler,
            IActorRef orderExecutorManager)
            : base(
                commandHandler,
                OrderBookState.Empty,
                Constants.OrderBookPersistenceId)
        {
            _orderExecutorManager = orderExecutorManager ?? throw new ArgumentNullException(nameof(orderExecutorManager));
        }

        protected override void OnPersist(IEvent persistedEvent)
        {
            if (persistedEvent is MatchedOrdersEvent matchedOrdersEvent)
            {
                foreach (var matchedOrdersMatch in matchedOrdersEvent.MatchedOrders.Matches)
                {
                    _orderExecutorManager.Tell(
                        new BeginOrderExecutionCommand(matchedOrdersMatch.Bid));

                    _orderExecutorManager.Tell(
                        new BeginOrderExecutionCommand(matchedOrdersMatch.Ask));
                }
            }

            base.OnPersist(persistedEvent);
        }
    }
}