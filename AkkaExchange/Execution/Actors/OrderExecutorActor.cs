using System;
using System.Collections.Generic;
using Akka.Actor;
using AkkaExchange.Shared.Actors;
using AkkaExchange.Execution.Commands;
using AkkaExchange.Execution.Events;
using AkkaExchange.Utils;

namespace AkkaExchange.Execution.Actors
{
    public class OrderExecutorActor : BaseActor<OrderExecutorState>
    {
        private readonly IOrderExecutor _orderExecutor;
        private readonly IActorRef _orderExecutorManager;

        public OrderExecutorActor(
            IOrderExecutor orderExecutor,
            ICommandHandler<OrderExecutorState> handler,
            IGlobalActorRefs globalActorRefs,
            IActorRef orderExecutorManager,
            OrderExecutorState defaultState) : 
                base(
                    handler, 
                    globalActorRefs, 
                    defaultState, 
                    defaultState.OrderExecutorId.ToString())
        {
            _orderExecutor = orderExecutor;
            _orderExecutorManager = orderExecutorManager ?? throw new ArgumentNullException(nameof(orderExecutorManager));
        }

        protected override void OnPersist(IEvent persistedEvent)
        {
            if (persistedEvent is BeginOrderExecutionEvent beginOrderExecutionEvent)
            {
                _orderExecutor
                    .Execute(beginOrderExecutionEvent.Match)
                    .PipeTo(
                        Self, 
                        Self, 
                        () => 
                            new UpdateOrderExecutionStatusCommand(
                                OrderExecutorStatus.Complete,
                                beginOrderExecutionEvent.Match,
                                beginOrderExecutionEvent.OrderExecutionId),
                        e => 
                            new UpdateOrderExecutionStatusCommand(
                                OrderExecutorStatus.Error,
                                beginOrderExecutionEvent.Match,
                                beginOrderExecutionEvent.OrderExecutionId)
                        );
            }

            if (persistedEvent is UpdateOrderExecutionStatusEvent updateOrderExecutionStatusEvent &&
                updateOrderExecutionStatusEvent.Status == OrderExecutorStatus.Complete)
            {
                _orderExecutorManager.Tell(
                    new UpdateOrderExecutionStatusCommand(
                        OrderExecutorStatus.Complete, 
                        updateOrderExecutionStatusEvent.Match,
                        updateOrderExecutionStatusEvent.OrderId));
            }

            base.OnPersist(persistedEvent);
        }
    }
}