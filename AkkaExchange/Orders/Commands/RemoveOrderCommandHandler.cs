using AkkaExchange.Orders.Events;
using AkkaExchange.State;
using System.Linq;

namespace AkkaExchange.Orders.Commands
{
    public class RemoveOrderCommandHandler : BaseCommandHandler<ExchangeActorState, RemoveOrderCommand>
    {
        protected override HandlerResult Handle(ExchangeActorState state, RemoveOrderCommand command)
        {
            if (state.Orders.Any(o => o.OrderId == command.OrderId))
            {
                return new HandlerResult(
                    new RemoveOrderEvent(
                        command.OrderId));
            }
            else
            {
                return HandlerResult.NotFound(command);
            }
        }
    }
}
