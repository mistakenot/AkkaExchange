using System;
using AkkaExchange.Utils;
using Moq;
using Reactive.Streams;
using Xunit;

namespace AkkaExchange.Tests.Utils
{
    public class ObserverSubscriberTests
    {
        private readonly Mock<IObserver<int>> _observerMock;
        private readonly Mock<ISubscription> _subscriptionMock;
        private readonly ObserverSubscriber<int> _subject;

        public ObserverSubscriberTests()
        {
            _observerMock = new Mock<IObserver<int>>();
            _subscriptionMock = new Mock<ISubscription>();
            _subject = new ObserverSubscriber<int>(_observerMock.Object);
        }

        [Fact]
        public void ObserverSubscriber_WhenOnSubscriber_CallsRequestOne()
        {
            _subject.OnSubscribe(_subscriptionMock.Object);

            _subscriptionMock.Verify(s => s.Request(1));
        }

        [Fact]
        public void ObserverSubscriber_OnSubscribeCalledTwice_Fails()
        {
            _subject.OnSubscribe(_subscriptionMock.Object);

            Assert.Throws<InvalidOperationException>(() => _subject.OnSubscribe(_subscriptionMock.Object));
        }

        [Fact]
        public void ObserverSubscriber_OnNext_CallsRequestOne()
        {
            _subject.OnSubscribe(_subscriptionMock.Object);
            _subject.OnNext(1);
            
            _subscriptionMock.Verify(s => s.Request(1), Times.Exactly(2));
            _observerMock.Verify(o => o.OnNext(1), Times.Once);
        }

        [Fact]
        public void ObserverSubscrier_OnComplete_CallsCancelAndComplete()
        {
            _subject.OnSubscribe(_subscriptionMock.Object);
            _subject.OnComplete();
            
            _subscriptionMock.Verify(s => s.Cancel(), Times.Once);
            _observerMock.Verify(s => s.OnCompleted(), Times.Once);
        }

        [Fact]
        public void ObserverSubscriber_OnError_CallsCancel()
        {
            var ex = new Exception("an_exception");
            _subject.OnSubscribe(_subscriptionMock.Object);
            _subject.OnError(ex);

            _subscriptionMock.Verify(s => s.Cancel(), Times.Once);
            _observerMock.Verify(o => o.OnError(It.Is<Exception>(e => e == ex && e.Message == ex.Message)), Times.Once);
        }
    }
}