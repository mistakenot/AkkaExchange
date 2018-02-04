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
using AkkaExchange.Shared.Queries;
using AkkaExchange.Utils;

namespace AkkaExchange
{
    public class AkkaExchangeQueries
    {
        public IObservable<ClientManagerState> ClientManagerState { get; }
        public IObservable<OrderBookState> OrderBookState { get; }
        public IObservable<PlacedOrderVolume> PlacedOrderVolumePerTenSeconds { get; }
        public IObservable<HandlerErrorEvent> HandlerErrorEvents { get; }
        public IActorRef HandlerErrorEventsSource { get; }

        public AkkaExchangeQueries(
            IObservable<ClientManagerState> clientManagerState,
            IObservable<OrderBookState> orderBookState, 
            IObservable<PlacedOrderVolume> placedOrderVolumePerMinute,
            IObservable<HandlerErrorEvent> handlerErrorEvents,
            IActorRef handlerErrorEventsSource)
        {
            PlacedOrderVolumePerTenSeconds = placedOrderVolumePerMinute;
            HandlerErrorEvents = handlerErrorEvents ?? throw new ArgumentNullException(nameof(handlerErrorEvents));
            HandlerErrorEventsSource = handlerErrorEventsSource ?? throw new ArgumentNullException(nameof(handlerErrorEventsSource));
            ClientManagerState = clientManagerState ?? throw new ArgumentNullException(nameof(clientManagerState));
            OrderBookState = orderBookState ?? throw new ArgumentNullException(nameof(orderBookState));
        }

        internal static AkkaExchangeQueries Create(
            IMaterializer materializer, 
            IEventsByPersistenceIdQuery eventsByPersistenceIdQuery)
        {
            var clientManagerQueryFactory = new StateQueryFactory<ClientManagerState>(
                eventsByPersistenceIdQuery, 
                materializer, 
                Client.ClientManagerState.Empty);

            var orderBookQueryFactory = new StateQueryFactory<OrderBookState>(
                eventsByPersistenceIdQuery, 
                materializer, 
                Orders.OrderBookState.Empty);

            var placedOrderVolumeFactory = new PlacedOrderVolumeQueryFactory(
                eventsByPersistenceIdQuery,
                materializer);
            
            var handlerErrorFactory = new HandlerErrorQueryFactory(materializer);
            var (source, observable) = handlerErrorFactory.Create();

            return new AkkaExchangeQueries(
                clientManagerQueryFactory.Create("client-manager"),
                orderBookQueryFactory.Create("order-book"),
                placedOrderVolumeFactory.Create("order-book", TimeSpan.FromSeconds(10)),
                observable,
                source);
        }
    }
}