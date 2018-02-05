using System;
using System.Collections.Immutable;
using AkkaExchange.Client;
using AkkaExchange.Client.Commands;
using AkkaExchange.Client.Events;
using Xunit;

namespace AkkaExchange.Tests.Client
{
    public class ClientManagerHandlerTests
    {
        [Fact]
        public void ClientManagerHandler_ReceivesEndConnectionCommand_Ok()
        {

            var clientId = Guid.Empty;
            var state = new ClientManagerState(
                ImmutableList<Guid>.Empty.Add(clientId));
                
            var actual = new ClientManagerHandler()
                .Handle(state, new EndConnectionCommand(clientId));

            Assert.True(actual.Success);
            var evnt = Assert.IsType<EndConnectionEvent>(actual.Event);
            Assert.Equal(clientId, evnt.ClientId);
        }
    }
}