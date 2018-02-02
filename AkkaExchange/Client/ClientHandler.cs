﻿using System;
using AkkaExchange.Client.Commands;
using AkkaExchange.Client.Events;
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

                return new HandlerResult(
                    new ExecuteOrderEvent(
                        executeOrderCommand.ClientId, 
                        executeOrderCommand.OrderCommand));
            }

            if (command is CompleteOrderCommand completeOrderCommand)
            {
                if (state.Status != ClientStatus.Connected)
                {
                    return new HandlerResult(
                        $"Client is not connected.");
                }

                return new HandlerResult(
                    new CompleteOrderEvent(
                        completeOrderCommand.Order.Side,
                        completeOrderCommand.Order.Amount));
            }

            return HandlerResult.NotHandled;
        }
    }
}