using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Akka.Actor;
using Akka.Persistence.Query;
using Akka.Streams;
using Akka.Streams.Dsl;
using AkkaExchange.Client;
using AkkaExchange.Client.Queries;
using AkkaExchange.Orders;
using AkkaExchange.Orders.Queries;
using AkkaExchange.Shared;
using AkkaExchange.Shared.Events;
using AkkaExchange.Shared.Extensions;
using AkkaExchange.Shared.Queries;
using AkkaExchange.Utils;

namespace AkkaExchange
{
    public interface IAkkaExchangeQueries
    {
        IOrderQueries Orders { get; }
        IClientQueries Clients { get; }
        IObservable<HandlerErrorEvent> HandlerErrorEvents { get; }
        
    }
    
    public class AkkaExchangeQueries : IAkkaExchangeQueries
    {
        public IOrderQueries Orders { get; }
        public IClientQueries Clients { get; }
        public IObservable<HandlerErrorEvent> HandlerErrorEvents { get; }
        public IActorRef HandlerErrorEventsSource { get; }

        public AkkaExchangeQueries(
            IOrderQueries orderQueries,
            IClientQueries clientQueries,
            IObservable<HandlerErrorEvent> handlerErrorEvents,
            IActorRef handlerErrorEventsSource)
        {
            Orders = orderQueries;
            Clients = clientQueries ?? throw new ArgumentNullException(nameof(clientQueries));
            HandlerErrorEvents = handlerErrorEvents ?? throw new ArgumentNullException(nameof(handlerErrorEvents));
            HandlerErrorEventsSource = handlerErrorEventsSource ?? throw new ArgumentNullException(nameof(handlerErrorEventsSource));
        }

        internal static AkkaExchangeQueries Create(
            IMaterializer materializer, 
            IEventsByPersistenceIdQuery eventsByPersistenceIdQuery)
        {
            var handlerErrorSubject = new Subject<HandlerErrorEvent>();
            var handlerErrorSubscriber = new ObserverSubscriber<HandlerErrorEvent>(handlerErrorSubject);
            var handlerErrorStreamSource = Source
                .ActorRef<HandlerErrorEvent>(100, OverflowStrategy.DropHead)
                .ToMaterialized(
                    Sink.FromSubscriber(handlerErrorSubscriber),
                    Keep.Left)
                .Run(materializer);

            // var handlerErrorFactory = new HandlerErrorQueryFactory(materializer);
            // var (handlerErrorObservable, handlerErrorSource) = handlerErrorFactory.Create();

            var orderQueries = OrderQueries.Create(
                "order-book",
                eventsByPersistenceIdQuery,
                materializer);

            var clientQueries = ClientQueries.Create(
                "client-manager",
                eventsByPersistenceIdQuery,
                materializer,
                handlerErrorSubject);

            return new AkkaExchangeQueries(
                orderQueries,
                clientQueries,
                handlerErrorSubject,
                handlerErrorStreamSource);
        }
    }
}