using System;
using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.Xunit2;
using AkkaExchange.Orders;
using AkkaExchange.Orders.Commands;
using AkkaExchange.Orders.Events;
using AkkaExchange.Orders.Extensions;
using Xunit;

namespace AkkaExchange.Tests.Orders
{
    public class OrderBookActorTests : TestKit
    {
        private readonly IActorRef _subject;
        private readonly TestProbe _probe;
        
        public OrderBookActorTests()
        {
            _probe = this.CreateTestProbe();
        }
        
        [Fact]
        public void ExchangeActor_ReceivesValidNewBidOrder_Ok()
        {
            Within(TimeSpan.FromSeconds(1), () =>
            {
                var msg = new NewOrderCommand(
                    new Order(Guid.Empty, 1m, 1m, OrderSide.Bid));
                
                _subject.Tell(msg, _probe.Ref);
                AwaitCondition(() => _probe.HasMessages);

                var evnt = _probe.ExpectMsg<NewOrderEvent>();

                Assert.Equal(msg.Order.Amount, evnt.Order.Amount);
                Assert.Equal(msg.Order.Price, evnt.Order.Price);
                Assert.Equal(msg.Order.Side, evnt.Order.Side);
            });
        }

        [Fact]
        public void ExchangeActor_ReceivesValidNewAskOrder_Ok()
        {
            Within(TimeSpan.FromSeconds(1), () =>
            {
                var newOrderCommand = new NewOrderCommand(
                    new Order(Guid.Empty, 1m, 1m, OrderSide.Ask));

                _subject.Tell(newOrderCommand, _probe.Ref);
                AwaitCondition(() => _probe.HasMessages);

                var @event = _probe.ExpectMsg<NewOrderEvent>();

                Assert.Equal(newOrderCommand.Order.Amount, @event.Order.Amount);
                Assert.Equal(newOrderCommand.Order.Price, @event.Order.Price);
                Assert.Equal(newOrderCommand.Order.Side, @event.Order.Side);
                Assert.NotEqual(Guid.Empty, @event.Order.OrderId);
            });
        }

        [Fact]
        public void ExchangeActor_ReceivesValidAmendOrder_Ok()
        {
            Within(TimeSpan.FromSeconds(2), () =>
            {
                var newOrderCommand = new NewOrderCommand(
                    new Order(Guid.Empty, 1m, 1m, OrderSide.Bid));

                _subject.Tell(newOrderCommand, _probe.Ref);
                AwaitCondition(() => _probe.HasMessages);

                var newOrderEvent = _probe.ExpectMsg<NewOrderEvent>();

                var amendOrderCommand = new AmendOrderCommand(
                    newOrderEvent.Order.OrderId, 
                    newOrderCommand.Order.WithAmount(2));

                _subject.Tell(amendOrderCommand, _probe.Ref);
                AwaitCondition(() => _probe.HasMessages);

                var amendOrderEvent = _probe.ExpectMsg<AmendOrderEvent>();
                Assert.Equal(amendOrderCommand.Order.Amount, amendOrderEvent.Order.Amount);
            });
        }

        [Fact]
        public void ExchangeActor_ReceivesValidRemoveOrder_Ok()
        {
            Within(TimeSpan.FromSeconds(2), () =>
            {
                var newOrderCommand = new NewOrderCommand(
                    new Order(Guid.Empty, 1m, 1m, OrderSide.Bid));

                _subject.Tell(newOrderCommand, _probe.Ref);
                AwaitCondition(() => _probe.HasMessages);

                var newOrderEvent = _probe.ExpectMsg<NewOrderEvent>();

                var removeOrderCommand = new RemoveOrderCommand(
                    newOrderEvent.Order.OrderId);

                _subject.Tell(removeOrderCommand, _probe.Ref);
                AwaitCondition(() => _probe.HasMessages);

                var removeOrderEvent = _probe.ExpectMsg<RemoveOrderEvent>();
                Assert.Equal(removeOrderCommand.OrderId, removeOrderEvent.OrderId);
            });
        }
    }
}
