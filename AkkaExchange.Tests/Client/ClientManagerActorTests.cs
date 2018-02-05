using System;
using System.Threading.Tasks;
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
    public class ClientManagerActorTests : TestKit
    {
        [Fact]
        public async Task ClientManagerActor_ReceivesEndConnectionEvent_ShutsdownChild_Ok()
        {
            var clientState = ClientFixture.ConnectedState;

            var clientManagerCommandHandlerMock = new Mock<ICommandHandler<ClientManagerState>>();
            clientManagerCommandHandlerMock
                .Setup(h => h.Handle(It.IsAny<ClientManagerState>(), It.IsAny<EndConnectionCommand>()))
                .Returns(new HandlerResult(new EndConnectionEvent(clientState.ClientId)));

            var props = Props.Create<ClientManagerActor>(
                clientManagerCommandHandlerMock.Object,
                Mock.Of<ICommandHandler<ClientState>>(),
                Mock.Of<IGlobalActorRefs>());

            var childProps = Props.Create<ClientActor>(
                Mock.Of<ICommandHandler<ClientState>>(),
                Mock.Of<IGlobalActorRefs>(),
                clientState);

            var subjectActor = Sys.ActorOf(props);

            await Task.Delay(10);
            
            var childActor = ActorOfAsTestActorRef<ClientActor>(
                childProps,
                subjectActor, 
                clientState.ClientId.ToString());
            
            var probe = CreateTestProbe();
            probe.Watch(childActor);
            
            subjectActor.Tell(
                new EndConnectionCommand(clientState.ClientId));
            
            probe.ExpectTerminated(childActor);
        }
    }
}