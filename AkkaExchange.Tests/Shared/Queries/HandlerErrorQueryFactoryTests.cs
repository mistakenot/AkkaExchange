using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Streams;
using Akka.TestKit.Xunit2;
using AkkaExchange.Shared.Events;
using AkkaExchange.Shared.Queries;
using AkkaExchange.Utils;
using Moq;
using Xunit;

namespace AkkaExchange.Tests.Shared.Queries
{
    public class HandlerErrorQueryFactoryTests : TestKit
    {
        [Fact]
        public void HandlerErrorQueryFactory_EmitsEvent_Ok()
        {
            using (var materializer = Sys.Materializer())
            {
                var subject = new HandlerErrorQueryFactory(materializer);
                var (actor, observable) = subject.Create();
                var msg = new HandlerErrorEvent("", HandlerResult.NotHandled);
                var subscriber = new Mock<IObserver<HandlerErrorEvent>>();
                var subscription = observable.Subscribe(subscriber.Object);
                
                actor.Tell(msg, ActorRefs.Nobody);
                
                subscriber.Verify(s => s.OnNext(It.IsAny<HandlerErrorEvent>()));
            }
        }
    }
}