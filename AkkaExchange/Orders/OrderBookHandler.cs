using System;
using System.Linq;
using AkkaExchange.Matching.Events;
using AkkaExchange.Orders.Commands;
using AkkaExchange.Orders.Events;
using AkkaExchange.Orders.Extensions;
using AkkaExchange.Utils;

namespace AkkaExchange.Orders
{
    public class OrderBookHandler : ICommandHandler<OrderBookState>
    {
        public HandlerResult Handle(OrderBookState state, ICommand command)
        {
            if (command is AmendOrderCommand amendOrderCommand)
            {
                var order = state.PendingOrders.FirstOrDefault(o => o.OrderId == amendOrderCommand.OrderId);

                if (order == null)
                {
                    return new HandlerResult($"Order Id {amendOrderCommand.OrderId} not found.");
                }

                return new HandlerResult(
                    new AmendOrderEvent(
                        amendOrderCommand.Order));
            }

            if (command is NewOrderCommand newOrderCommand)
            {
                return new HandlerResult(
                    new NewOrderEvent(
                        newOrderCommand.Order.WithOrderId(Guid.NewGuid())));
            }

            if (command is RemoveOrderCommand removeOrderCommand)
            {
                if (state.PendingOrders.Any(o => o.OrderId == removeOrderCommand.OrderId))
                {
                    return new HandlerResult(
                        new RemoveOrderEvent(
                            removeOrderCommand.OrderId));
                }
                else
                {
                    return HandlerResult.NotFound(command);
                }
            }

            if (command is BeginMatchOrdersCommand)
            {
                if (state.IsMatching)
                {
                    return new HandlerResult(
                        "Order book is alreay matching.");
                }
                else
                {
                    return new HandlerResult(
                        new BeginMatchOrdersEvent(state.PendingOrders));
                }
            }

            if (command is EndMatchOrdersCommand endMatchOrdersCommand)
            {
                if (!state.IsMatching)
                {
                    return new HandlerResult(
                        "Order book is already matching.");
                }
                else
                {
                    return new HandlerResult(
                        new EndMatchOrdersEvent(endMatchOrdersCommand.MatchedOrders));
                }
            }

            return HandlerResult.NotHandled;
        }
    }
}