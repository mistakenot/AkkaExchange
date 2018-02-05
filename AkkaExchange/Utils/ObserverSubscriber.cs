using System;
using Reactive.Streams;

namespace AkkaExchange.Utils
{
    public class ObserverSubscriber<T> : ISubscriber<T>, IDisposable
    {
        private readonly IObserver<T> _observer;
        private ISubscription _subscription;

        public ObserverSubscriber(IObserver<T> observer)
        {
            _observer = observer ?? throw new ArgumentNullException(nameof(observer));
        }

        public void OnSubscribe(ISubscription subscription)
        {
            if (_subscription != null)
            {
                throw new InvalidOperationException("Multiple calls to OnSubscribe() are not allowed.");
            }

            _subscription = subscription;
            _subscription.Request(1);
        }

        public void OnComplete() =>_observer.OnCompleted();

        public void OnError(Exception cause) => _observer.OnError(cause);

        public void OnNext(T element)
        {
            if (_subscription == null)
            {
                throw new InvalidOperationException("OnSubscribe must be called before OnNext.");
            }

            _observer.OnNext(element);
            _subscription.Request(1);
        }

        public void Dispose() => _subscription?.Cancel();
    }
}