using System;
using System.Reactive.Linq;
using Akka.Actor;
using Akka.Persistence.Query;
using Akka.Streams;
using Akka.Streams.Dsl;
using AkkaExchange.Client;
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

        IObservable<HandlerErrorEvent> HandlerErrorEvents { get; }
        IObservable<ClientManagerState> ClientManagerState { get; }
    }
    
    public class AkkaExchangeQueries : IAkkaExchangeQueries
    {
        public IOrderQueries Orders { get; }

        public IObservable<HandlerErrorEvent> HandlerErrorEvents { get; }
        public IObservable<ClientManagerState> ClientManagerState { get; }
        public IActorRef HandlerErrorEventsSource { get; }

        public AkkaExchangeQueries(
            IOrderQueries orderQueries,
            IObservable<ClientManagerState> clientManagerState,
            IObservable<HandlerErrorEvent> handlerErrorEvents,
            IActorRef handlerErrorEventsSource)
        {
            Orders = orderQueries;
            HandlerErrorEvents = handlerErrorEvents ?? throw new ArgumentNullException(nameof(handlerErrorEvents));
            HandlerErrorEventsSource = handlerErrorEventsSource ?? throw new ArgumentNullException(nameof(handlerErrorEventsSource));
            ClientManagerState = clientManagerState ?? throw new ArgumentNullException(nameof(clientManagerState));
        }

        internal static AkkaExchangeQueries Create(
            IMaterializer materializer, 
            IEventsByPersistenceIdQuery eventsByPersistenceIdQuery)
        {
            var clientManagerQueryFactory = new StateQueryFactory<ClientManagerState>(
                eventsByPersistenceIdQuery, 
                materializer, 
                Client.ClientManagerState.Empty);
            
            var handlerErrorFactory = new HandlerErrorQueryFactory(materializer);
            var (handlerErrorObservable, handlerErrorSource) = handlerErrorFactory.Create();

            var orderQueries = OrderQueries.Create(
                "order-book",
                eventsByPersistenceIdQuery,
                materializer);

            return new AkkaExchangeQueries(
                orderQueries,
                clientManagerQueryFactory.Create("client-manager"),
                handlerErrorObservable,
                handlerErrorSource);
        }
    }
}