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
        private readonly IDictionary<Guid, IDisposable> _orderExecutionEventSubscriptions;

        public OrderExecutorActor(
            IOrderExecutor orderExecutor,
            ICommandHandler<OrderExecutorState> handler,
            IActorRef orderExecutorManager,
            OrderExecutorState defaultState) : base(handler, defaultState, defaultState.OrderId.ToString())
        {
            _orderExecutor = orderExecutor;
            _orderExecutorManager = orderExecutorManager ?? throw new ArgumentNullException(nameof(orderExecutorManager));
            _orderExecutionEventSubscriptions = new Dictionary<Guid, IDisposable>();
        }

        protected override void OnPersist(IEvent persistedEvent)
        {
            if (persistedEvent is BeginOrderExecutionEvent beginOrderExecutionEvent)
            {
                var orderId = beginOrderExecutionEvent.Order.OrderId;
                var executionEvents = _orderExecutor.Execute(beginOrderExecutionEvent.Order);
                var self = Self;
                var executionEventsSubscription = executionEvents.Subscribe(e =>
                {
                    self.Tell(new UpdateOrderExecutionStatusCommand(e, orderId), self);
                });

                _orderExecutionEventSubscriptions.Add(
                    orderId, 
                    executionEventsSubscription);
            }

            if (persistedEvent is UpdateOrderExecutionStatusEvent updateOrderExecutionStatusEvent &&
                updateOrderExecutionStatusEvent.Status == OrderExecutorStatus.Complete)
            {
                _orderExecutorManager.Tell(
                    new UpdateOrderExecutionStatusCommand(
                        OrderExecutorStatus.Complete, 
                        updateOrderExecutionStatusEvent.OrderId));
            }

            base.OnPersist(persistedEvent);
        }

        public override void AroundPostStop()
        {
            foreach (var orderExecutionEventSubscription in _orderExecutionEventSubscriptions)
            {
                orderExecutionEventSubscription.Value.Dispose();
            }

            base.AroundPostStop();
        }
    }
}