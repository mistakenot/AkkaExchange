using System;
using System.Collections.Generic;
using AkkaExchange.Actors;
using AkkaExchange.Execution.Commands;
using AkkaExchange.Execution.Events;

namespace AkkaExchange.Execution.Actors
{
    public class OrderExecutorActor : BaseActor<OrderExecutorState>
    {
        private readonly IOrderExecutor _orderExecutor;
        private readonly IDictionary<Guid, IDisposable> _orderExecutionEventSubscriptions;

        public OrderExecutorActor(
            IOrderExecutor orderExecutor,
            ICommandHandler<OrderExecutorState> handler, 
            OrderExecutorState defaultState) : base(handler, defaultState, defaultState.OrderId.ToString())
        {
            _orderExecutor = orderExecutor;
            _orderExecutionEventSubscriptions = new Dictionary<Guid, IDisposable>();
        }

        protected override void OnPersist(IEvent persistedEvent)
        {
            if (persistedEvent is BeginOrderExecutionEvent beginOrderExecutionEvent)
            {
                var orderId = beginOrderExecutionEvent.Order.OrderId;
                var executionEvents = _orderExecutor.Execute(beginOrderExecutionEvent.Order);
                var executionEventsSubscription = executionEvents.Subscribe(e =>
                {
                    Self.Tell(new UpdateOrderExecutionStatusCommand(e, orderId), Self);
                });

                _orderExecutionEventSubscriptions.Add(
                    orderId, 
                    executionEventsSubscription);
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