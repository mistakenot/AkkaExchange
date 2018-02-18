using System;
using Reactive.Streams;

namespace AkkaExchange.Utils
{
    public class PublisherObservable<T> : IObservable<T>
    {
        private readonly IPublisher<T> _publisher;

        public PublisherObservable(IPublisher<T> publisher)
        {
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        }
        
        public IDisposable Subscribe(IObserver<T> observer)
        {
            var subscriber = new ObserverSubscriber<T>(observer);
            _publisher.Subscribe(subscriber);
            return subscriber;
        }
    }   
}