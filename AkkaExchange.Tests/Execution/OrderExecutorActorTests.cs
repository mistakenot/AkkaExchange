using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly OrderMatch _order;

        public OrderExecutorActorTests()
        {
            _orderExecutorMock = new Mock<IOrderExecutor>();
            _commandHandler = new Mock<ICommandHandler<OrderExecutorState>>();
            _defaultState = new OrderExecutorState(_order);
            _orderExecutorManager = CreateTestProbe();
            _executorSubject = new Subject<OrderExecutorStatus>();

            var bid =  new PlacedOrder(
                new Order(
                    Guid.NewGuid(), 1m, 1m, OrderSide.Bid),
                DateTime.UtcNow,
                Guid.NewGuid());

            var ask =  new PlacedOrder(
                new Order(
                    Guid.NewGuid(), 1m, 1m, OrderSide.Ask),
                    DateTime.UtcNow,
                    Guid.NewGuid());
            
            _order = new OrderMatch(bid, ask);

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
                            Guid.NewGuid(),
                            _order,
                            OrderExecutorStatus.Complete)));

            _orderExecutorMock
                .Setup(m => m.Execute(It.IsAny<OrderMatch>()))
                .Returns(Task.FromResult(0));
        }

        [Fact]
        public void OrderExecutorActor_CompletedExecution_NotifiesManager()
        {
            Within(TimeSpan.FromSeconds(3), () =>
            {
                var props = Props.Create<OrderExecutorActor>(
                    _orderExecutorMock.Object,
                    _commandHandler.Object,
                    new GlobalActorRefs(),
                    _orderExecutorManager.Ref,
                    _defaultState);

                var subject = Sys.ActorOf(props);

                subject.Tell(new BeginOrderExecutionCommand(_order), ActorRefs.Nobody);
                
                _orderExecutorManager.ExpectMsg<UpdateOrderExecutionStatusCommand>(TimeSpan.FromSeconds(3));
            });
        }
    }
}