using System;
using AkkaExchange.Execution;
using AkkaExchange.Execution.Commands;
using AkkaExchange.Execution.Events;
using AkkaExchange.Orders;
using AkkaExchange.Tests.Orders;
using AkkaExchange.Utils;
using Xunit;

namespace AkkaExchange.Tests.Execution
{
    public class OrderExecutorHandlerTests : OrderFixture
    {
        private readonly OrderExecutorHandler _subject;
        private readonly PlacedOrder _order;

        public OrderExecutorHandlerTests()
        {
            _order = new PlacedOrder(
                new Order(
                    Guid.NewGuid(), 1m, 1m, OrderSide.Bid));

            _subject = new OrderExecutorHandler();
        }

        [Fact]
        public void OrderExecutorHandler_ReceivesValidBeginOrderExecutionCommand_Ok()
        {
            var result = _subject.Handle(
                new OrderExecutorState(_order.OrderId),
                new BeginOrderExecutionCommand(_order));

            var evnt = AssertSuccess<BeginOrderExecutionEvent>(result);
            Assert.Equal(_order.OrderId, evnt.Order.OrderId);
        }

        [Fact]
        public void OrderExecutorHandler_UpdatesStatusPendingToInProgress_Ok()
            => AssertStateChangeSuccess(OrderExecutorStatus.Pending, OrderExecutorStatus.InProgress);

        [Fact]
        public void OrderExecutorHandler_UpdatesStatusInProgressToComplete_Ok()
            => AssertStateChangeSuccess(OrderExecutorStatus.InProgress, OrderExecutorStatus.Complete);

        [Fact]
        public void OrderExecutorHandler_UpdatesStatusInProgressToFailure_Ok()
            => AssertStateChangeSuccess(OrderExecutorStatus.InProgress, OrderExecutorStatus.Error);

        [Fact]
        public void OrderExecutorHandler_UpdateStatusInProgressToPending_Fail() 
            => AssertStateChangeFailure(OrderExecutorStatus.InProgress, OrderExecutorStatus.Pending);

        private void AssertStateChangeSuccess(
            OrderExecutorStatus initialStatus, 
            OrderExecutorStatus resultStatus)
        {
            var result = ChangeState(initialStatus, resultStatus);
            var evnt = AssertSuccess<UpdateOrderExecutionStatusEvent>(result);
            Assert.Equal(_order.OrderId, evnt.OrderId);
            Assert.Equal(resultStatus, evnt.Status);
        }

        private void AssertStateChangeFailure(
            OrderExecutorStatus initialStatus,
            OrderExecutorStatus resultStatus)
        {
            var result = ChangeState(initialStatus, resultStatus);
            Assert.False(result.Success);
            Assert.NotEmpty(result.Errors);
        }

        private HandlerResult ChangeState(OrderExecutorStatus initialStatus, OrderExecutorStatus resultStatus) 
            => _subject.Handle(
                new OrderExecutorState(_order.OrderId, initialStatus),
                new UpdateOrderExecutionStatusCommand(resultStatus, _order.OrderId));
    }
}