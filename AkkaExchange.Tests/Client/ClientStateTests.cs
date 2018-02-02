using System;
using AkkaExchange.Client;
using AkkaExchange.Client.Events;
using AkkaExchange.Orders;
using AkkaExchange.Orders.Commands;
using Xunit;

namespace AkkaExchange.Tests.Client
{
    public class ClientStateTests
    {
        private ClientState _subject;
        private readonly Guid _clientId;

        public ClientStateTests()
        {
            _clientId = Guid.NewGuid();
            _subject = new ClientState(_clientId);
        }

        [Fact]
        public void ClientState_ReceivesStartConnectionEvent_Ok()
        {
            var evnt = new StartConnectionEvent(_clientId, DateTime.UtcNow);

            _subject = _subject.Update(evnt);

            Assert.Equal(ClientStatus.Connected, _subject.Status);
            Assert.NotNull(_subject.StartedAt);
        }

        [Fact]
        public void ClientState_ReceivesEndConnectionEvent_Ok()
        {
            ClientState_ReceivesStartConnectionEvent_Ok();

            var evnt = new EndConnectionEvent(_clientId, DateTime.UtcNow);

            _subject = _subject.Update(evnt);

            Assert.Equal(ClientStatus.Disconnected, _subject.Status);
            Assert.NotNull(_subject.EndedAt);
        }

        [Fact]
        public void ClientState_ReceivesExecuteOrderEvent_Ok()
        {
            ClientState_ReceivesStartConnectionEvent_Ok();

            var order = new Order(_clientId, 0m, 0m, OrderSide.Bid);
            var evnt = new ExecuteOrderEvent(
                _clientId,
                new NewOrderCommand(order));
            
            _subject = _subject.Update(evnt);

            Assert.Single(_subject.OrderCommandHistory);
        }

        [Fact]
        public void ClientState_ReceivesCompleteOrderEvent_UpdatesBalanceOk()
        {
            ClientState_ReceivesStartConnectionEvent_Ok();

            var evnt = new CompleteOrderEvent(OrderSide.Bid, 10m);

            _subject = _subject.Update(evnt);

            Assert.Equal(90, _subject.Balance);
        }
    }
}