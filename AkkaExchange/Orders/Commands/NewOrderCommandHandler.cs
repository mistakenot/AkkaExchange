using AkkaExchange.Orders.Events;
using AkkaExchange.Orders.Extensions;
using AkkaExchange.State;
using System;
using AkkaExchange.Utils;

namespace AkkaExchange.Orders.Commands
{
    public class NewOrderCommandHandler : BaseCommandHandler<ExchangeActorState, NewOrderCommand>
    {
        protected override HandlerResult Handle(ExchangeActorState state, NewOrderCommand command)
        {
            return new HandlerResult(
                new NewOrderEvent(
                    command.Order.WithOrderId(Guid.NewGuid())));
        }
    }
}
