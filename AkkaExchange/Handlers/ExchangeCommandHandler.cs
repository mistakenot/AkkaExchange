using System;
using System.Linq;
using AkkaExchange.Events;
using AkkaExchange.Commands;
using AkkaExchange.State;

namespace AkkaExchange.Handlers
{
    public class ExchangeCommandHandler 
        : ICommandHandler<ExchangeActorState>
    {
        public HandlerResult Handle(ExchangeActorState state, object command)
        {
            switch (command)
            {
                case NewOrderCommand newOrderCommand:
                    return new HandlerResult(
                        new NewOrderEvent(
                            new Order(
                                Guid.NewGuid(),
                                newOrderCommand.OrderDetails)));

                case AmendOrderCommand amendOrderCommand:
                    var order = state.Orders.FirstOrDefault(o => o.OrderId == amendOrderCommand.OrderId);

                    if (order == null)
                    {
                        return new HandlerResult($"Order Id {amendOrderCommand.OrderId} not found.");
                    }

                    return new HandlerResult(
                        new AmendOrderEvent(
                            amendOrderCommand.OrderId,
                            amendOrderCommand.OrderDetails));

                case BeginMatchOrdersCommand beginMatchOrdersCommand:
                    return new HandlerResult(
                        new BeginMatchOrdersEvent(
                            state.Orders));

                case RemoveOrderCommand removeOrderCommand:
                    if (state.Orders.Any(o => o.OrderId == removeOrderCommand.OrderId))
                    {
                        return new HandlerResult(
                            new RemoveOrderEvent(
                                removeOrderCommand.OrderId));
                    }
                    else
                    {
                        return HandlerResult.NotFound(command);
                    }
                default:
                    return HandlerResult.NotFound(command);
            }
        }
    }
}
