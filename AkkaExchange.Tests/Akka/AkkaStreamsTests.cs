using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Streams;
using Akka.Streams.Dsl;
using Akka.TestKit.Xunit2;
using AkkaExchange.Shared.Events;
using AkkaExchange.Shared.Queries;
using AkkaExchange.Utils;
using Moq;
using Reactive.Streams;
using Xunit;

namespace AkkaExchange.Tests.Akka
{
    public class AkkaStreamsTests : TestKit
    {
        [Fact]
        public void AkkaStreams_ActorSourceActorSink_Works()
        {
            using (var materializer = Sys.Materializer())
            {
                var probe = CreateTestProbe();
                var source = Source.ActorRef<HandlerErrorEvent>(10, OverflowStrategy.DropNew);
                var sink = Sink.ActorRef<HandlerErrorEvent>(probe, PoisonPill.Instance);
                var graph = source.ToMaterialized(sink, Keep.Left);
                var actor = graph.Run(materializer);

                var msg = new HandlerErrorEvent("", HandlerResult.NotHandled);

                actor.Tell(msg, ActorRefs.Nobody);

                var actual = probe.ExpectMsg<HandlerErrorEvent>(TimeSpan.FromSeconds(3));
                
                Assert.Equal(HandlerResult.NotHandled, actual.Result);
            }
        }

        // [Fact] Wairing for a Stack Overflow response.
        public void AkkaStreams_ActorSourcePublisherSink_Works()
        {
            using (var materializer = Sys.Materializer())
            {
                var probe = CreateTestProbe();
                var source = Source.ActorRef<HandlerErrorEvent>(10, OverflowStrategy.DropNew);
                var subscriber = new Mock<ISubscriber<HandlerErrorEvent>>();
                var sink = Sink.FromSubscriber<HandlerErrorEvent>(subscriber.Object);
                var graph = source.ToMaterialized(sink, Keep.Both);
                var (actor, publisher) = graph.Run(materializer);
                
                // publisher.Subscribe(subscriber.Object);
                subscriber.Verify(s => s.OnSubscribe(It.IsAny<ISubscription>()));

                var evnt = new HandlerErrorEvent("", HandlerResult.NotHandled);
                actor.Tell(evnt, ActorRefs.Nobody);

                base.AwaitCondition(() =>
                {
                    try
                    {
                        subscriber.Verify(s => s.OnNext(It.IsAny<HandlerErrorEvent>()));
                        return true;
                    }
                    catch(MockException)
                    {
                        return false;
                    }
                });
            }
        }

        [Fact]
        public async Task AkkaStreams_ActorSourceForeachSink_Works()
        {
            using (var materializer = Sys.Materializer())
            {
                var source = Source.ActorRef<HandlerErrorEvent>(10, OverflowStrategy.DropNew);
                var observer = new Mock<IObserver<HandlerErrorEvent>>();
                var sink = Sink.ForEach<HandlerErrorEvent>(e => observer.Object.OnNext(e));
                var graph = source.ToMaterialized(sink, Keep.Both);
                var (actor, task) = graph.Run(materializer);

                var msg = new HandlerErrorEvent("", HandlerResult.NotHandled);

                actor.Tell(msg, ActorRefs.Nobody);

                await Task.WhenAny(Task.Delay(1000), task);

                observer.Verify(o => o.OnNext(It.IsAny<HandlerErrorEvent>()));
            }
        }
    }
}