using System;
using System.Threading.Tasks;
using Akka;
using Akka.Streams;
using Akka.Streams.Dsl;

namespace AkkaExchange.Utils
{
    public class SourceObservable<T> : IObservable<T>
    {
        private readonly Source<T, NotUsed> _source;
        private readonly IMaterializer _materializer;

        public SourceObservable(
            Source<T, NotUsed> source, 
            IMaterializer materializer)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _materializer = materializer ?? throw new ArgumentNullException(nameof(materializer));
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            var task = _source.RunForeach(
                observer.OnNext,
                _materializer);

            return new Subscription(task);
        }

        class Subscription : IDisposable
        {
            private readonly Task _subscriptionTask;

            public Subscription(Task subscriptionTask)
            {
                _subscriptionTask = subscriptionTask;
            }

            public void Dispose()
            {
                _subscriptionTask.Dispose();
            }
        }
    }
}