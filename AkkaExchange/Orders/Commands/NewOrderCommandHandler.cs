using System;
using AkkaExchange.Orders.Events;
using AkkaExchange.State;

namespace AkkaExchange.Orders.Commands
{
    public class NewOrderCommandHandler : BaseCommandHandler<ExchangeActorState, NewOrderCommand>
    {
        protected override HandlerResult Handle(ExchangeActorState state, NewOrderCommand command)
        {
            return new HandlerResult(
                new NewOrderEvent(
                    new Order(
                        Guid.NewGuid(),
                        command.OrderDetails)));
        }
    }
}
