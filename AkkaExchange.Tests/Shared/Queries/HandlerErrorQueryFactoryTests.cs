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
        public async Task HandlerErrorQueryFactory_EmitsEvent_Ok()
        {
            using (var materializer = Sys.Materializer())
            {
                var subject = new HandlerErrorQueryFactory(materializer);
                var (observable, source) = subject.Create();
                var msg = new HandlerErrorEvent("", HandlerResult.NotHandled);
                var subscriber = new Mock<IObserver<HandlerErrorEvent>>();
                var subscription = observable.Subscribe(subscriber.Object);
                
                // Graph is run async, so we need to give it a chance
                //  to warm up before asserting anything.
                source.Tell(msg, ActorRefs.Nobody);
                source.Tell(msg, ActorRefs.Nobody);
                await Task.Delay(10);
                subscriber.Verify(s => s.OnNext(It.IsAny<HandlerErrorEvent>()), Times.Exactly(2));
            }
        }
    }
}