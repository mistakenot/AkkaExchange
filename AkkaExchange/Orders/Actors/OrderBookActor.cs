using AkkaExchange.Actors;
using AkkaExchange.Orders.Commands;
using AkkaExchange.State;
using AkkaExchange.Utils;

namespace AkkaExchange.Orders.Actors
{
    /// <summary>
    /// Maintains the current open order book.
    /// </summary>
    public class OrderBookActor : BaseActor<ExchangeActorState>
    {
        public OrderBookActor()
            : base(
                OrderCommandHandler.Instance,
                new ExchangeActorState(),
                Constants.OrderBookPersistenceId)
        {
        }
    }
}