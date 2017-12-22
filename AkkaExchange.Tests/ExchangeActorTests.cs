using System;
using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.Xunit2;
using AkkaExchange.Actors;
using AkkaExchange.Events;
using AkkaExchange.Commands;
using AkkaExchange.State;
using Xunit;

namespace AkkaExchange.Tests
{
    public class ExchangeActorTests : TestKit
    {
        private readonly IActorRef _subject;
        private readonly TestProbe _probe;
        
        public ExchangeActorTests()
        {
            _subject = this.Sys.ActorOf<ExchangeActor>();
            _probe = this.CreateTestProbe();
        }
        
        [Fact]
        public void ExchangeActor_ReceivesValidNewBidOrder_Ok()
        {
            Within(TimeSpan.FromSeconds(1), () =>
            {
                var msg = new NewOrderCommand(
                    new OrderDetails(1m, 1m, OrderStateSide.Bid));
                
                _subject.Tell(msg, _probe.Ref);
                AwaitCondition(() => _probe.HasMessages);

                var evnt = _probe.ExpectMsg<NewOrderEvent>();

                Assert.Equal(msg.OrderDetails.Amount, evnt.Order.Details.Amount);
                Assert.Equal(msg.OrderDetails.Price, evnt.Order.Details.Price);
                Assert.Equal(msg.OrderDetails.Side, evnt.Order.Details.Side);
            });
        }

        [Fact]
        public void ExchangeActor_ReceivesValidNewAskOrder_Ok()
        {
            Within(TimeSpan.FromSeconds(1), () =>
            {
                var newOrderCommand = new NewOrderCommand(
                    new OrderDetails(1m, 1m, OrderStateSide.Ask));

                _subject.Tell(newOrderCommand, _probe.Ref);
                AwaitCondition(() => _probe.HasMessages);

                var @event = _probe.ExpectMsg<NewOrderEvent>();

                Assert.Equal(newOrderCommand.OrderDetails.Amount, @event.Order.Details.Amount);
                Assert.Equal(newOrderCommand.OrderDetails.Price, @event.Order.Details.Price);
                Assert.Equal(newOrderCommand.OrderDetails.Side, @event.Order.Details.Side);
                Assert.NotEqual(Guid.Empty, @event.Order.OrderId);
            });
        }

        [Fact]
        public void ExchangeActor_ReceivesValidAmendOrder_Ok()
        {
            Within(TimeSpan.FromSeconds(2), () =>
            {
                var newOrderCommand = new NewOrderCommand(
                    new OrderDetails(1m, 1m, OrderStateSide.Bid));

                _subject.Tell(newOrderCommand, _probe.Ref);
                AwaitCondition(() => _probe.HasMessages);

                var newOrderEvent = _probe.ExpectMsg<NewOrderEvent>();

                var amendOrderCommand = new AmendOrderCommand(
                    newOrderEvent.Order.OrderId, 
                    newOrderCommand.OrderDetails.WithAmount(2));

                _subject.Tell(amendOrderCommand, _probe.Ref);
                AwaitCondition(() => _probe.HasMessages);

                var amendOrderEvent = _probe.ExpectMsg<AmendOrderEvent>();
                Assert.Equal(amendOrderCommand.OrderDetails.Amount, amendOrderEvent.OrderDetails.Amount);
            });
        }

        [Fact]
        public void ExchangeActor_ReceivesValidRemoveOrder_Ok()
        {
            Within(TimeSpan.FromSeconds(2), () =>
            {
                var newOrderCommand = new NewOrderCommand(
                    new OrderDetails(1m, 1m, OrderStateSide.Bid));

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
