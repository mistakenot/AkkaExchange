using System;
using Reactive.Streams;

namespace AkkaExchange.Utils
{
    // TODO Double check the reactive streams spec.
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

        public void OnComplete()
        {
            if (_subscription == null)
            {
                throw new InvalidOperationException("Cannot call OnComplete before OnSubscribe.");
            }

            _observer.OnCompleted();
            _subscription.Cancel();
        }

        public void OnError(Exception cause)
        {
            if (_subscription == null)
            {
                throw new InvalidOperationException("Cannot call OnError before OnSubscribe.");
            }

            _observer.OnError(cause);
            _subscription.Cancel();
        }

        public void OnNext(T element)
        {
            if (_subscription == null)
            {
                throw new InvalidOperationException("OnSubscribe must be called before OnNext.");
            }

            _observer.OnNext(element);
            _subscription.Request(1);
        }

        public void Dispose()
        {
            _subscription?.Cancel();
        }
    }
}