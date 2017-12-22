using System.Linq;
using AkkaExchange.Orders.Events;
using AkkaExchange.State;

namespace AkkaExchange.Orders.Commands
{
    public class AmendOrderCommandHandler : BaseCommandHandler<ExchangeActorState, AmendOrderCommand>
    {
        protected override HandlerResult Handle(ExchangeActorState state, AmendOrderCommand command)
        {
            var order = state.Orders.FirstOrDefault(o => o.OrderId == command.OrderId);

            if (order == null)
            {
                return new HandlerResult($"Order Id {command.OrderId} not found.");
            }

            return new HandlerResult(
                new AmendOrderEvent(
                    command.OrderId,
                    command.OrderDetails));
        }
    }
}
