using System;
using System.Diagnostics;
using AkkaExchange.Client.Commands;
using AkkaExchange.Client.Events;
using AkkaExchange.Orders;
using AkkaExchange.Orders.Commands;
using AkkaExchange.Utils;

namespace AkkaExchange.Client
{
    public class ClientHandler : ICommandHandler<ClientState>
    {
        public HandlerResult Handle(ClientState state, ICommand command)
        {
            if (command is StartConnectionCommand startConnectionCommand)
            {
                if (state.Status == ClientStatus.Pending)
                {
                    return new HandlerResult(
                        new StartConnectionEvent(
                            startConnectionCommand.ClientId,
                             DateTime.UtcNow));
                }
                else
                {
                    return new HandlerResult(
                        $"Client is in wrong state for start connection event: {state.Status}");
                }
            }

            if (command is EndConnectionCommand endConnectionCommand)
            {
                if (endConnectionCommand.ClientId != state.ClientId)
                {
                    return new HandlerResult(
                        $"Incorrect client Id is incorrect.");
                }

                if (state.Status == ClientStatus.Connected)
                {
                    return new HandlerResult(
                        new EndConnectionEvent(endConnectionCommand.ClientId));
                }
                else
                {
                    return new HandlerResult(
                        $"Incorrect status.");
                }
            }

            if (command is ExecuteOrderCommand executeOrderCommand)
            {
                if (executeOrderCommand.ClientId != state.ClientId)
                {
                    return new HandlerResult(
                        $"Incorrect client Id is incorrect.");
                }

                if (state.Status != ClientStatus.Connected)
                {
                    return new HandlerResult(
                        $"Client is not connected.");
                }

                if (executeOrderCommand.OrderCommand is NewOrderCommand bidOrderCommand &&
                    bidOrderCommand.Order.Side == OrderSide.Bid &&
                    bidOrderCommand.Order.TotalPrice() > state.Balance)
                {
                    return new HandlerResult(
                        $"Balance too low.");
                }

                if (executeOrderCommand.OrderCommand is NewOrderCommand askOrderCommand &&
                    askOrderCommand.Order.Side == OrderSide.Ask &&
                    askOrderCommand.Order.Amount > state.Amount)
                {
                    return new HandlerResult(
                        $"Asset amount too low.");
                }
                
                return new HandlerResult(
                    new ExecuteOrderEvent(
                        executeOrderCommand.ClientId, 
                        executeOrderCommand.OrderCommand));
            }

            if (command is Client.Commands.CompleteOrderCommand completeOrderCommand)
            {
                if (state.Status != ClientStatus.Connected)
                {
                    return new HandlerResult(
                        $"Client is not connected.");
                }

                return new HandlerResult(
                    new CompleteOrderEvent(
                        completeOrderCommand.Order));
            }

            return HandlerResult.NotHandled;
        }
    }
}