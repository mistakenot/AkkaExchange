using System;
using System.Linq;
using AkkaExchange.Orders.Commands;
using AkkaExchange.Orders.Events;
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
                var order = state.OpenOrders.FirstOrDefault(o => o.OrderId == amendOrderCommand.OrderId);

                if (order == null)
                {
                    return new HandlerResult($"Order Id {amendOrderCommand.OrderId} not found.");
                }

                return new HandlerResult(
                    new AmendOrderEvent(
                        new PlacedOrder(
                            amendOrderCommand.Order, 
                            order.PlacedAt, 
                            order.OrderId)));
            }

            if (command is NewOrderCommand newOrderCommand)
            {
                return new HandlerResult(
                    new NewOrderEvent(
                        new PlacedOrder(
                            newOrderCommand.Order,
                            DateTime.UtcNow,
                            Guid.NewGuid())));
            }

            if (command is RemoveOrderCommand removeOrderCommand)
            {
                if (state.OpenOrders.Any(o => o.OrderId == removeOrderCommand.OrderId))
                {
                    return new HandlerResult(
                        new RemoveOrderEvent(
                            removeOrderCommand.OrderId));
                }
                else
                {
                    return new HandlerResult($"Order ID {removeOrderCommand.OrderId} not found.");
                }
            }

            if (command is MatchOrdersCommand)
            {
                var result = _orderMatcher.Match(state.OpenOrders);

                return new HandlerResult(
                    new MatchedOrdersEvent(result));
            }

            if (command is CompleteOrdersCommand completeOrderCommand)
            {
                if (state.ExecutingOrders.Any(o => o.OrderId == completeOrderCommand.Match.Bid.OrderId) &&
                    state.ExecutingOrders.Any(o => o.OrderId == completeOrderCommand.Match.Ask.OrderId))
                {
                    var bid = state.ExecutingOrders.Single(o => o.OrderId == completeOrderCommand.Match.Bid.OrderId);
                    var ask = state.ExecutingOrders.Single(o => o.OrderId == completeOrderCommand.Match.Ask.OrderId);

                    return new HandlerResult(
                        new CompleteOrderEvent(bid, ask));
                }
                else
                {
                    return new HandlerResult(
                        $"Order id {completeOrderCommand.Match.Bid.OrderId} or {completeOrderCommand.Match.Ask.OrderId} not found in executing orders.");
                }
            }

            return HandlerResult.NotHandled(command);
        }
    }
}