using System;
using Akka.Persistence.Query;
using Akka.Streams;
using AkkaExchange.Client;
using AkkaExchange.Orders;
using AkkaExchange.Shared.Queries;

namespace AkkaExchange
{
    public class AkkaExchangeQueries
    {
        public IObservable<ClientManagerState> ClientManagerState { get; }
        public IObservable<OrderBookState> OrderBookState { get; }

        public AkkaExchangeQueries(
            IObservable<ClientManagerState> clientManagerState,
            IObservable<OrderBookState> orderBookState)
        {
            ClientManagerState = clientManagerState ?? throw new ArgumentNullException(nameof(clientManagerState));
            OrderBookState = orderBookState ?? throw new ArgumentNullException(nameof(orderBookState));
        }

        internal static AkkaExchangeQueries Create(IMaterializer materializer, IEventsByPersistenceIdQuery eventsByPersistenceIdQuery)
        {
            var clientManagerQueryFactory = new StateQueryFactory<ClientManagerState>(
                eventsByPersistenceIdQuery, 
                materializer, 
                Client.ClientManagerState.Empty);

            var orderBookQueryFactory = new StateQueryFactory<OrderBookState>(
                eventsByPersistenceIdQuery, 
                materializer, 
                Orders.OrderBookState.Empty);

            return new AkkaExchangeQueries(
                clientManagerQueryFactory.Create("client-manager"),
                orderBookQueryFactory.Create("order-book"));
        }
    }
}