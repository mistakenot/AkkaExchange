using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Streams;
using Akka.Streams.Dsl;
using AkkaExchange.Shared.Events;
using AkkaExchange.Utils;
using Reactive.Streams;

namespace AkkaExchange.Shared.Queries
{
    public class HandlerErrorQueryFactory
    {
        private readonly IMaterializer _materializer;

        public HandlerErrorQueryFactory(IMaterializer materializer)
        {
            _materializer = materializer;
        }

        public (IActorRef, IObservable<HandlerErrorEvent>) Create()
        {
            var subject = new Subject<HandlerErrorEvent>();
            var source = Source.ActorRef<HandlerErrorEvent>(0, OverflowStrategy.DropNew);
            var sink = Sink.FromSubscriber(new Subscriber(subject));
            var graph = source.ToMaterialized(sink, Keep.Left);
            var actor = graph.Run(_materializer);

            return (actor, subject);
        }

        class Subscriber : ISubscriber<HandlerErrorEvent>
        {
            private readonly IObserver<HandlerErrorEvent> _observer;
            private ISubscription _subscription;

            public Subscriber(IObserver<HandlerErrorEvent> observer)
            {
                _observer = observer;
            }

            public void OnComplete() => _observer.OnCompleted();
            public void OnError(Exception cause) => _observer.OnError(cause);
            public void OnNext(HandlerErrorEvent element)
            {
                _observer.OnNext(element);
                _subscription?.Request(1);
            }

            public void OnSubscribe(ISubscription subscription)
            {
                _subscription = subscription;
                _subscription.Request(1);
            }
        }
    }
}