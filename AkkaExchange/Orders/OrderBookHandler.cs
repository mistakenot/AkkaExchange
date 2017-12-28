using System;
using System.Linq;
using AkkaExchange.Orders.Commands;
using AkkaExchange.Orders.Events;
using AkkaExchange.Orders.Extensions;
using AkkaExchange.Utils;

namespace AkkaExchange.Orders
{
    public class OrderBookHandler : ICommandHandler<OrderBookState>
    {
        private readonly IOrderMatcher _orderMatcher;

        public OrderBookHandler(IOrderMatcher orderMatcher)
        {
            _orderMatcher = orderMatcher;
        }

        public HandlerResult Handle(OrderBookState state, ICommand command)
        {
            if (command is AmendOrderCommand amendOrderCommand)
            {
                var order = state.PendingOrders.FirstOrDefault(o => o.Details.OrderId == amendOrderCommand.OrderId);

                if (order == null)
                {
                    return new HandlerResult($"Order Id {amendOrderCommand.OrderId} not found.");
                }

                return new HandlerResult(
                    new AmendOrderEvent(
                        new PlacedOrder(amendOrderCommand.Order, order.PlacedAt)));
            }

            if (command is NewOrderCommand newOrderCommand)
            {
                return new HandlerResult(
                    new NewOrderEvent(
                        new PlacedOrder(
                            newOrderCommand.Order.WithOrderId(Guid.NewGuid()),
                            DateTime.UtcNow)));
            }

            if (command is RemoveOrderCommand removeOrderCommand)
            {
                if (state.PendingOrders.Any(o => o.Details.OrderId == removeOrderCommand.OrderId))
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

            if (command is MatchOrdersCommand)
            {
                
            }

            return HandlerResult.NotHandled;
        }
    }
}