using System;
using System.Linq;
using AkkaExchange.Client;
using AkkaExchange.Client.Events;
using Xunit;

namespace AkkaExchange.Tests.Client
{
    public class ClientManagerStateTests
    {
        private ClientManagerState _subject;

        public ClientManagerStateTests()
        {
            _subject = ClientManagerState.Empty;
        }

        [Fact]
        public void ClientManagerState_ReceiveStartConnectionEvent_Ok()
        {
            var evnt = new StartConnectionEvent(Guid.NewGuid(), DateTime.UtcNow);
            _subject = _subject.Update(evnt);
            Assert.Single(_subject.ClientIds);
            Assert.Equal(evnt.ClientId, _subject.ClientIds.First());
        }

        [Fact]
        public void ClientManagerState_ReceiveEndConnectionEvent_Ok()
        {
            ClientManagerState_ReceiveStartConnectionEvent_Ok();

            var clientId = _subject.ClientIds.Single();
            var evnt = new EndConnectionEvent(clientId, DateTime.Now);

            _subject = _subject.Update(evnt);

            Assert.Empty(_subject.ClientIds);
        }
    }
}