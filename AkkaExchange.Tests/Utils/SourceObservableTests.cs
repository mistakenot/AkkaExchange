using System;
using Akka.Streams;
using Akka.Streams.Dsl;
using Akka.TestKit.Xunit2;
using AkkaExchange.Utils;
using Moq;
using Xunit;

namespace AkkaExchange.Tests.Akka
{
    public class SourceObservableTests : TestKit
    {
        [Fact]
        public void SourceObservable_CanReceiveEventsFromStream_Ok()
        {
            var source = Source.From(new [] {0, 1, 2});
            var subject = new SourceObservable<int>(source, Sys.Materializer());
            var observerMock = new Mock<IObserver<int>>();

            var subscription = subject.Subscribe(observerMock.Object);
            
            AwaitAssert(() => observerMock.Verify(o => o.OnNext(0), Times.Once));
            AwaitAssert(() => observerMock.Verify(o => o.OnNext(1), Times.Once));
            AwaitAssert(() => observerMock.Verify(o => o.OnNext(2), Times.Once));
            AwaitAssert(() => observerMock.Verify(o => o.OnCompleted(), Times.Once));
        }

        [Fact]
        public void SourceObservable_CanPropagateErrorsFromStream_Ok()
        {
            var ex = new Exception("A failure");
            var source = Source.Failed<int>(ex);
            var subject = new SourceObservable<int>(source, Sys.Materializer());
            var observerMock = new Mock<IObserver<int>>();

            var subscription = subject.Subscribe(observerMock.Object);

            AwaitAssert(() => observerMock.Verify(o => o.OnError(ex), Times.Once));
        }
    }
}