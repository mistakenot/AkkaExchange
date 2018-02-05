using System;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using AkkaExchange.Client;
using AkkaExchange.Client.Actors;
using AkkaExchange.Client.Commands;
using AkkaExchange.Client.Events;
using AkkaExchange.Utils;
using Moq;
using Xunit;

namespace AkkaExchange.Tests.Client
{
    public class ClientActorTests : TestKit
    {
        [Fact]
        public void ClientActor_ReceivesEndConnectionCommand_SendEndConnectionEventToManager()
        {
            var initialState = ClientFixture.ConnectedState;

            var commandHandlerMock = new Mock<ICommandHandler<ClientState>>();
            commandHandlerMock
                .Setup(h => h.Handle(initialState, It.IsAny<EndConnectionCommand>()))
                .Returns(new HandlerResult(new EndConnectionEvent(initialState.ClientId)));

            var probe = CreateTestProbe();
            var globalActorRefs = new Mock<IGlobalActorRefs>();
            globalActorRefs
                .SetupGet(r => r.ClientManager)
                .Returns(probe.Ref);

            var props = Props.Create<ClientActor>(
                commandHandlerMock.Object,
                globalActorRefs.Object,
                initialState);

            var subject = Sys.ActorOf(props);

            subject.Tell(new EndConnectionCommand(initialState.ClientId), ActorRefs.Nobody);

            var msg = probe.ExpectMsg<EndConnectionCommand>(TimeSpan.FromSeconds(1));
            Assert.Equal(initialState.ClientId, msg.ClientId);
        }
    }
}