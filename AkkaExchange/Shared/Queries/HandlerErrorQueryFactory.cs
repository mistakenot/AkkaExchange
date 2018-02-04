using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
            var source = Source.ActorRef<HandlerErrorEvent>(10, OverflowStrategy.DropNew);
            var sink = Sink.AsPublisher<HandlerErrorEvent>(false);
            var graph = source.ToMaterialized(sink, Keep.Both);
            var (actor, publisher) = graph.Run(_materializer);
            var subject = new Subject<HandlerErrorEvent>();
            var subscriber = new Subscriber(subject);

            publisher.Subscribe(subscriber);

            return (actor, subject);
        }
        
        /// Converts between an Akka subscriptions and Rx observables.
        class Subscriber : ISubscriber<HandlerErrorEvent>
        {
            private readonly IObserver<HandlerErrorEvent> _next;

            public Subscriber(IObserver<HandlerErrorEvent> next)
            {
                _next = next;
            }

            public void OnComplete() => _next.OnCompleted();
            public void OnError(Exception cause) => _next.OnError(cause);
            public void OnNext(HandlerErrorEvent element) => _next.OnNext(element);
            public void OnSubscribe(ISubscription subscription) {}
        }
    }
}