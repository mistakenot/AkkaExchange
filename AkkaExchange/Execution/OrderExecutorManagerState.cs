using System;
using System.Collections.Immutable;
using AkkaExchange.Execution.Events;
using AkkaExchange.State;

namespace AkkaExchange.Execution
{
    public class OrderExecutorManagerState : IState<OrderExecutorManagerState>
    {
        public IImmutableDictionary<Guid, OrderExecutionState> ExecutingObservables { get; }

        public OrderExecutorManagerState(
            IImmutableDictionary<Guid, OrderExecutionState> executingObservables)
        {
            ExecutingObservables =
                executingObservables ?? throw new ArgumentNullException(nameof(executingObservables));
        }

        public OrderExecutorManagerState Update(IEvent evnt)
        {
            if (evnt is BeginOrderExecutionEvent beginOrderExecutionEvent)
            {
                return new OrderExecutorManagerState(
                    ExecutingObservables.Add(
                        beginOrderExecutionEvent.Order.OrderId, 
                        new OrderEx));
            }
        }
    }
}