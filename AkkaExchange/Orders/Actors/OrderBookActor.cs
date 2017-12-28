using AkkaExchange.Shared.Actors;
using AkkaExchange.Utils;

namespace AkkaExchange.Orders.Actors
{
    /// <summary>
    /// Maintains the current open order book.
    /// </summary>
    public class OrderBookActor : BaseActor<OrderBookState>
    {
        public OrderBookActor(
            ICommandHandler<OrderBookState> commandHandler)
            : base(
                commandHandler,
                OrderBookState.Empty,
                Constants.OrderBookPersistenceId)
        {
        }
    }
}