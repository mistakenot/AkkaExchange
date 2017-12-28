using Akka.Actor;
using Akka.DI.Core;
using AkkaExchange.Execution.Commands;
using AkkaExchange.Orders.Commands;
using AkkaExchange.Utils;

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
                var orderId = beginOrderExecutionCommand.Order.OrderId.ToString();

                if (Context.Child(orderId) == ActorRefs.Nobody)
                {
                    var props = Props.Create<OrderExecutorActor>(
                        _orderExecutor,
                        _handler,
                        Self,
                        new OrderExecutorState(beginOrderExecutionCommand.Order.OrderId));

                    var child = Context.ActorOf(props, orderId);

                    child.Tell(beginOrderExecutionCommand, Sender);
                }
            }

            if (message is UpdateOrderExecutionStatusCommand updateOrderExecutionStatusCommand &&
                updateOrderExecutionStatusCommand.Status == OrderExecutorStatus.Complete)
            {
                var orderId = updateOrderExecutionStatusCommand.OrderId.ToString();

                if (Context.Child(orderId) != ActorRefs.Nobody)
                {
                    var child = Context.Child(orderId);
                    Context.Stop(child);

                    _globalActorRefs.OrderBook.Tell(
                        new RemoveOrderCommand(updateOrderExecutionStatusCommand.OrderId),
                        Self);
                }
            }
        }
    }
}
