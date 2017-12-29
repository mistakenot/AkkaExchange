using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AkkaExchange.Orders;
using AkkaExchange.Orders.Commands;
using AkkaExchange.Orders.Events;
using AkkaExchange.Orders.Extensions;
using Moq;
using Xunit;

namespace AkkaExchange.Tests.Orders
{
    public class OrderBookHandlerTests : OrderFixture
    {
        private readonly Mock<IOrderMatcher> _matcherMock;
        private readonly OrderBookHandler _subject;
        private readonly Order _order;

        public OrderBookHandlerTests()
        {
            _matcherMock = new Mock<IOrderMatcher>();
            _subject = new OrderBookHandler(_matcherMock.Object);
            _order = new Order(Guid.NewGuid(), 1m, 1m, OrderSide.Bid);
        }

        [Fact]
        public void OrderBookHandler_ReceivesValidNewOrderCommand_HandlesOk()
        {
            var result = _subject.Handle(
                OrderBookState.Empty,
                new NewOrderCommand(_order));

            AssertSuccess<NewOrderEvent>(result);
        }

        [Fact]
        public void OrderBookHandler_ReceivesValidAmendOrderCommand_HandlesOk()
        {
            var placedOrder = new PlacedOrder(_order);
            var result = _subject.Handle(
                OrderBookState.Empty.Update(new NewOrderEvent(placedOrder)),
                new AmendOrderCommand(placedOrder.OrderId, placedOrder.WithAmount(2m)));

            var evnt = AssertSuccess<AmendOrderEvent>(result);
            Assert.Equal(placedOrder.OrderId, evnt.Order.OrderId);
            Assert.Equal(2m, evnt.Order.Amount);
        }

        [Fact]
        public void OrderBookHandler_ReceivesValidMatchOrderCommand_CallsOrderMatcher()
        {
            _matcherMock
                .Setup(m => m.Match(It.IsAny<IEnumerable<PlacedOrder>>()))
                .Returns(new OrderMatchResult(Enumerable.Empty<OrderMatch>()));

            var result = _subject.Handle(
                OrderBookState.Empty,
                new MatchOrdersCommand());

            var evnt = AssertSuccess<MatchedOrdersEvent>(result);
            Assert.Empty(evnt.MatchedOrders.Matches);
        }

        [Fact]
        public void OrderBookHandler_ReceivesValidRemoveOrderCommand_HandlesOk()
        {
            var placedOrder = new PlacedOrder(_order);
            var result = _subject.Handle(
                OrderBookState.Empty.Update(new NewOrderEvent(placedOrder)),
                new RemoveOrderCommand(placedOrder.OrderId));

            var evnt = AssertSuccess<RemoveOrderEvent>(result);
            Assert.Equal(placedOrder.OrderId, evnt.OrderId);
        }

        [Fact]
        public void OrderBookHandler_ReceivesValidCompleteOrderCommand_HandlesOk()
        {
            var placedOrder = new PlacedOrder(_order);
            var result = _subject.Handle(
                new OrderBookState(
                    ImmutableList<PlacedOrder>.Empty,
                    ImmutableList<PlacedOrder>.Empty.Add(placedOrder),
                    ImmutableList<PlacedOrder>.Empty),
                new CompleteOrderCommand(placedOrder.OrderId));

            var evnt = AssertSuccess<CompleteOrderEvent>(result);
            Assert.Equal(placedOrder.OrderId, evnt.OrderId);
        }
    }
}