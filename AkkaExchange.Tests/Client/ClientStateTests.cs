using System;
using AkkaExchange.Client;
using AkkaExchange.Client.Events;
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
    }
}