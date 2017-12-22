using AkkaExchange.Actors;
using AkkaExchange.Orders.Commands;
using AkkaExchange.State;

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
                "exchange-actor")
        {
        }
    }
}