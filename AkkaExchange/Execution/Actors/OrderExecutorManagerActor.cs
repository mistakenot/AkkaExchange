using System;
using System.Collections.Immutable;
using Akka.DI.Core;
using AkkaExchange.Shared.Actors;
using AkkaExchange.Execution.Events;

namespace AkkaExchange.Execution.Actors
{
    public class OrderExecutorManagerActor : BaseActor<OrderExecutorManagerState>
    {
        public OrderExecutorManagerActor(
            string persistenceId) 
            : base(null, OrderExecutorManagerState.Empty, persistenceId)
        {
        }

        protected override void OnPersist(IEvent persistedEvent)
        {
            if (persistedEvent is BeginOrderExecutionEvent beginOrderExecutionEvent)
            {
                var props = Context.DI().Props<OrderExecutorActor>();
                var name = beginOrderExecutionEvent.Order.OrderId.ToString();
                var child = Context.ActorOf(props, name);
            }

            base.OnPersist(persistedEvent);
        }
    }

    public class OrderExecutorManagerState : IState<OrderExecutorManagerState>
    {
        public IImmutableDictionary<Guid, OrderExecutorActor> ExecutingOrders { get; }

        public OrderExecutorManagerState(IImmutableDictionary<Guid, OrderExecutorActor> executingOrders)
        {
            ExecutingOrders = executingOrders ?? throw new ArgumentNullException(nameof(executingOrders));
        }

        public static OrderExecutorManagerState Empty = 
            new OrderExecutorManagerState(
                ImmutableDictionary<Guid, OrderExecutorActor>.Empty);

        public OrderExecutorManagerState Update(IEvent evnt)
        {
            throw new NotImplementedException();
        }
    }
}
