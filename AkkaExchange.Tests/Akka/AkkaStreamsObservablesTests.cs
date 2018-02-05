using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Akka.Streams;
using Akka.Streams.Dsl;
using Akka.Streams.TestKit;
using Akka.TestKit.Xunit2;
using AkkaExchange.Utils;
using Moq;
using Reactive.Streams;
using Xunit;

namespace AkkaExchange.Tests.Akka
{
    public class AkkaStreamsObservablesTests : TestKit
    {
        private readonly IEnumerable<int> _values;
        private readonly IPublisher<int> _publisher;

        public AkkaStreamsObservablesTests()
        {
            _values = Enumerable.Range(0, 3);
            _publisher = Source
                .From(_values)
                .ToMaterialized(
                    Sink.Publisher<int>(),
                    Keep.Right)
                .Run(
                    Sys.Materializer());
        }

        [Fact]
        public void AkkaStreams_SendsAllMessages_Ok()
        {
            var probe = this.CreateManualSubscriberProbe<int>();

            _publisher.Subscribe(probe);

            var subscription = probe.ExpectSubscription();
            subscription.Request(3);

            probe.ExpectNextN(_values);
            probe.ExpectComplete();
        }

        [Fact]
        public void AkkaStreamsObservable_SendsAllMessages_Ok()
        {
            var observable = new PublisherObservable<int>(_publisher);
            var actual = observable
                .ToEnumerable()
                .ToArray();

            Assert.Equal(_values, actual);
        }

        [Fact]
        public void AkkaStreamsObservable_PropagatesErrors_Ok()
        {
            var expected = new Exception();
            var publisher = Source
                .Failed<int>(expected)
                .ToMaterialized(
                    Sink.Publisher<int>(),
                    Keep.Right)
                .Run(
                    Sys.Materializer());
            
            Exception actual = null;

            var observable = new PublisherObservable<int>(publisher);
            var subscription = observable.Subscribe<int>(i => {}, e => actual = e, () => {});
            
            AwaitCondition(() => expected == actual, TimeSpan.FromSeconds(3));
        }

        [Fact]
        public void AkkaStreamsObservable_DisposeCallsCancel_Ok()
        {
            var subscriptionMock = new Mock<ISubscription>();
            var observer = new ObserverSubscriber<int>(
                Mock.Of<IObserver<int>>());
            
            observer.OnSubscribe(subscriptionMock.Object);
            observer.Dispose();

            subscriptionMock.Verify(s => s.Cancel());
        }
    }
}