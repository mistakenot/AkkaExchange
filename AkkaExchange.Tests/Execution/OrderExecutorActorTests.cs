using System;
using AkkaExchange.Execution;
using AkkaExchange.Execution.Actors;
using AkkaExchange.Execution.Commands;
using AkkaExchange.Execution.Events;
using Moq;
using Xunit;
using Props = Akka.Actor.Props;

namespace AkkaExchange.Tests.Execution
{
    public class OrderExecutorActorTests : AkkaExchangeActorTestFixture
    {
        private readonly IOrderExecutor _orderExecutorMock;
        private readonly ICommandHandler<OrderExecutorState> _commandHandler;
        private readonly OrderExecutorState _defaultState;

        public OrderExecutorActorTests()
        {
            _orderExecutorMock = Mock.Of<IOrderExecutor>();
            _commandHandler = Mock.Of<ICommandHandler<OrderExecutorState>>();
            _defaultState = new OrderExecutorState(Guid.NewGuid());
        }

        // [Fact]
        public void OrderExecutorActor_ReceivesBeginOrderExecutionCommand_CreatesEvent()
        {
            Within(TimeSpan.FromSeconds(1), () =>
            {
                var props = Props.Create<OrderExecutorActor>(
                    _orderExecutorMock,
                    _commandHandler,
                    _defaultState);

                var subject = Sys.ActorOf(props);

                subject.Tell(
                    new BeginOrderExecutionCommand(),
                    Probe);

                WaitForOne<BeginOrderExecutionEvent>();
            });
        }
    }
}