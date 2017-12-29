using System;
using System.Reactive.Subjects;
using System.Threading;
using Akka.Actor;
using Akka.TestKit;
using AkkaExchange.Execution;
using AkkaExchange.Execution.Actors;
using AkkaExchange.Execution.Commands;
using AkkaExchange.Execution.Events;
using AkkaExchange.Orders;
using AkkaExchange.Orders.Commands;
using AkkaExchange.Utils;
using Moq;
using Xunit;
using Props = Akka.Actor.Props;

namespace AkkaExchange.Tests.Execution
{
    public class OrderExecutorActorTests : AkkaExchangeActorTestFixture
    {
        private readonly Mock<IOrderExecutor> _orderExecutorMock;
        private readonly Mock<ICommandHandler<OrderExecutorState>> _commandHandler;
        private readonly OrderExecutorState _defaultState;
        private readonly TestProbe _orderExecutorManager;
        private readonly Subject<OrderExecutorStatus> _executorSubject;
        private readonly PlacedOrder _order;

        public OrderExecutorActorTests()
        {
            _orderExecutorMock = new Mock<IOrderExecutor>();
            _commandHandler = new Mock<ICommandHandler<OrderExecutorState>>();
            _defaultState = new OrderExecutorState(Guid.NewGuid());
            _orderExecutorManager = CreateTestProbe();
            _executorSubject = new Subject<OrderExecutorStatus>();
            _order = new PlacedOrder(
                new Order(
                    Guid.NewGuid(), 1m, 1m, OrderSide.Ask),
                DateTime.UtcNow,
                _defaultState.OrderId);

            _commandHandler
                .Setup(m => m.Handle(
                    It.IsAny<OrderExecutorState>(),
                    It.IsAny<BeginOrderExecutionCommand>()))
                .Returns(new HandlerResult(new BeginOrderExecutionEvent(_order)));
            _commandHandler
                .Setup(m => m.Handle(
                    It.IsAny<OrderExecutorState>(),
                    It.IsAny<UpdateOrderExecutionStatusCommand>()))
                .Returns(
                    new HandlerResult(
                        new UpdateOrderExecutionStatusEvent(
                            _order.OrderId,
                            OrderExecutorStatus.InProgress)));

            _orderExecutorMock
                .Setup(m => m.Execute(It.IsAny<PlacedOrder>()))
                .Returns(_executorSubject);
        }

        // [Fact]
        public void OrderExecutorActor_CompletedExecution_NotifiesManager()
        {
            Within(TimeSpan.FromSeconds(1), () =>
            {
                var props = Props.Create<OrderExecutorActor>(
                    _orderExecutorMock.Object,
                    _commandHandler.Object,
                    _orderExecutorManager.Ref,
                    _defaultState);

                var subject = Sys.ActorOf(props);

                subject.Tell(new BeginOrderExecutionCommand(_order), ActorRefs.Nobody);

                Thread.Sleep(5000);

                _executorSubject.OnNext(OrderExecutorStatus.Complete);
                
                Thread.Sleep(5000000);
                _orderExecutorManager.ExpectMsg<UpdateOrderExecutionStatusCommand>(TimeSpan.MaxValue);
            });
        }
    }
}