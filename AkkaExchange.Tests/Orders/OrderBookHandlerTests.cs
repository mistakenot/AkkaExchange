using System;
using AkkaExchange.Orders;
using AkkaExchange.Orders.Commands;
using AkkaExchange.Orders.Events;
using AkkaExchange.Utils;
using Moq;
using Xunit;

namespace AkkaExchange.Tests.Orders
{
    public class OrderBookHandlerTests
    {
        private readonly Mock<IOrderMatcher> _matcherMock;
        private readonly OrderBookHandler _subject;
        private readonly Order _order;

        public OrderBookHandlerTests()
        {
            _matcherMock = new Mock<IOrderMatcher>();
            _subject = new OrderBookHandler(_matcherMock.Object);
            _order = new Order(Guid.NewGuid(), Guid.NewGuid(), 1m, 1m, OrderSide.Bid);
        }

        [Fact]
        public void OrderBookHandler_ReceivesValidNewOrderCommand_HandlesOk()
        {
            var result = _subject.Handle(
                OrderBookState.Empty,
                new NewOrderCommand(_order));

            AssertSuccess(result);
            var evnt = Assert.IsType<NewOrderEvent>(result.Event);
            Assert.Equal(_order.OrderId, evnt.Order.Details.OrderId);
        }

        private static void AssertSuccess(HandlerResult result)
        {
            Assert.True(result.Success);
            Assert.True(result.WasHandled);
            Assert.Empty(result.Errors);
        }
    }
}
