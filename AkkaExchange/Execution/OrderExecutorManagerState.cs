using System;
using System.Collections.Immutable;
using AkkaExchange.Execution.Events;

namespace AkkaExchange.Execution
{
    public class OrderExecutorManagerState : IState<OrderExecutorManagerState>
    {
        public IImmutableDictionary<Guid, OrderExecutorState> ExecutingObservables { get; }

        public OrderExecutorManagerState(
            IImmutableDictionary<Guid, OrderExecutorState> executingObservables)
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
                        beginOrderExecutionEvent.OrderExecutionId,
                        new OrderExecutorState(
                            beginOrderExecutionEvent.OrderExecutionId,
                            beginOrderExecutionEvent.Match,
                        OrderExecutorStatus.InProgress)));
            }

            return this;
        }
    }
}