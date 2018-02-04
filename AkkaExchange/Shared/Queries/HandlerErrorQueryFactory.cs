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

        public HandlerErrorQuery Create()
        {
            var source = Source.ActorRef<HandlerErrorEvent>(10, OverflowStrategy.DropNew);
            var subject = new Subject<HandlerErrorEvent>();
            var sink = Sink.ForEach<HandlerErrorEvent>(subject.OnNext);
            var graph = source.ToMaterialized(sink, Keep.Both);
            var (actor, task) = graph.Run(_materializer);

            return new HandlerErrorQuery(actor, task, subject);
        }
    }

    public class HandlerErrorQuery : IDisposable
    {

        public IActorRef Source { get; }
        public IObservable<HandlerErrorEvent> Observable { get; }
        private readonly Task _task;

        public HandlerErrorQuery(
            IActorRef source, 
            Task task, 
            IObservable<HandlerErrorEvent> observable)
        {
            Source = source;
            Observable = observable;
            
            _task = task;
        }

        public void Dispose()
        {
            _task.Dispose();
        }
    }
}