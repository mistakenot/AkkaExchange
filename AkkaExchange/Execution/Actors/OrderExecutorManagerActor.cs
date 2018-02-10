using Akka.Actor;
using AkkaExchange.Execution.Commands;
using AkkaExchange.Orders.Commands;
using AkkaExchange.Utils;
using System;

namespace AkkaExchange.Execution.Actors
{
    public class OrderExecutorManagerActor : UntypedActor
    {
        private readonly IOrderExecutor _orderExecutor;
        private readonly IGlobalActorRefs _globalActorRefs;
        private readonly ICommandHandler<OrderExecutorState> _handler;

        public OrderExecutorManagerActor(
            IOrderExecutor orderExecutor,
            IGlobalActorRefs globalActorRefs,
            ICommandHandler<OrderExecutorState> handler)
        {
            _orderExecutor = orderExecutor;
            _globalActorRefs = globalActorRefs;
            _handler = handler;
        }

        protected override void OnReceive(object message)
        {
            if (message is BeginOrderExecutionCommand beginOrderExecutionCommand)
            {
                var state = new OrderExecutorState(beginOrderExecutionCommand.Match);

                var props = Props.Create<OrderExecutorActor>(
                    _orderExecutor,
                    _handler,
                    _globalActorRefs,
                    Self,
                    state);

                var child = Context.ActorOf(props, state.OrderExecutorId.ToString());

                child.Tell(beginOrderExecutionCommand, Sender);
            }

            if (message is UpdateOrderExecutionStatusCommand updateOrderExecutionStatusCommand &&
                updateOrderExecutionStatusCommand.Status == OrderExecutorStatus.Complete)
            {
                var orderExecutionId = updateOrderExecutionStatusCommand.OrderExecutionId.ToString();

                if (Context.Child(orderExecutionId) != ActorRefs.Nobody)
                {
                    var child = Context.Child(orderExecutionId);
                    Context.Stop(child);

                    _globalActorRefs.OrderBook.Tell(
                        new CompleteOrdersCommand(
                            updateOrderExecutionStatusCommand.Match),
                        Self);
                }
            }
        }
    }
}
