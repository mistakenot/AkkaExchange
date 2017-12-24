using System;
using System.Collections.Generic;
using System.Linq;
using AkkaExchange.Utils;

namespace AkkaExchange
{
    public interface ICommandHandler<in TState>
    {
        bool CanHandle(object command);

        HandlerResult Handle(TState state, object command);
    }

    public abstract class BaseCommandHandler<TState, TCommand> : ICommandHandler<TState>
    {
        public bool CanHandle(object command)
            => command is TCommand;

        public HandlerResult Handle(TState state, object command)
        {
            if (!(state is TState))
            {
                throw new InvalidOperationException("State is wrong type.");
            }

            if (!(command is TCommand))
            {
                throw new InvalidOperationException("Command is wrong type.");
            }

            return Handle(state, (TCommand) command);
        }

        protected abstract HandlerResult Handle(TState state, TCommand command);
    }

    public class CommandHandlerCollection<TState> : ICommandHandler<TState>
    {
        public IEnumerable<ICommandHandler<TState>> CommandHandlers { get; }

        public CommandHandlerCollection(IEnumerable<ICommandHandler<TState>> commandHandlers)
        {
            CommandHandlers = commandHandlers;
        }
        
        public bool CanHandle(object command) 
            => CommandHandlers.Any(ch => ch.CanHandle(command));

        public HandlerResult Handle(TState state, object command)
        {
            foreach (var handler in CommandHandlers)
            {
                if (handler.CanHandle(command))
                {
                    return handler.Handle(state, command);
                }
            }

            return HandlerResult.NotFound(command);
        }
    }
}