using AkkaExchange.Actors;
using AkkaExchange.Matching.Events;
using AkkaExchange.Orders.Commands;
using AkkaExchange.Utils;

namespace AkkaExchange.Orders.Actors
{
    /// <summary>
    /// Maintains the current open order book.
    /// </summary>
    public class OrderBookActor : BaseActor<OrderBookState>
    {
        private readonly IOrderMatcher _orderMatcher;

        public OrderBookActor(
            ICommandHandler<OrderBookState> commandHandler,
            IOrderMatcher orderMatcher)
            : base(
                commandHandler,
                OrderBookState.Empty,
                Constants.OrderBookPersistenceId)
        {
            _orderMatcher = orderMatcher;
        }

        protected override void OnPersist(IEvent persistedEvent)
        {
            if (persistedEvent is BeginMatchOrdersEvent beginMatchOrdersEvent)
            {
                var result = _orderMatcher.Match(beginMatchOrdersEvent.Orders);
                var command = new EndMatchOrdersCommand(result);

                Self.Tell(command, Self);
            }

            base.OnPersist(persistedEvent);
        }
    }
}