using System;
using System.Collections.Immutable;
using System.Linq;
using AkkaExchange.Client;
using AkkaExchange.Client.Commands;
using AkkaExchange.Orders.Commands;
using AkkaExchange.Orders.Extensions;
using Xunit;

namespace AkkaExchange.Tests.Client
{
    public class ClientHandlerTests : ClientFixture
    {
        private readonly ClientState _state;
        private readonly ClientHandler _subject;

        public ClientHandlerTests()
        {
            _subject = new ClientHandler();
        }

        [Fact]
        public void ClientHandler_ReceivesExecuteOrderCommand_NotEnoughMoney()
        {
            var command = new ExecuteOrderCommand(
                Guid.Empty,
                new NewOrderCommand(UnplacedBidOrder.WithAmount(1000)));

            var actual = _subject.Handle(
                ConnectedState,
                command);

            Assert.False(actual.Success);
            Assert.Single(actual.Errors);
            Assert.Equal("Balance too low.", actual.Errors.Single());
        }
    }
}