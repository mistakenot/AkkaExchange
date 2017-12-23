using System;
using Akka.Persistence;

namespace AkkaExchange.Actors
{
    public class BaseActor<TState> : UntypedPersistentActor
        where TState : class, IState<TState>
    {
        public override string PersistenceId { get; }

        public BaseActor(
            ICommandHandler<TState> handler, 
            TState defaultState, 
            string persistenceId)
        {
            PersistenceId = persistenceId;

            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            _state = defaultState ?? throw new ArgumentNullException(nameof(defaultState));
        }

        private readonly ICommandHandler<TState> _handler;
        private TState _state;

        protected override void OnCommand(object message)
        {
            var handlerResult = _handler.Handle(_state, message);

            if (handlerResult.Success)
            {
                Persist(handlerResult.Event, OnPersist);
            }
            else
            {
                // Send errors to client.
            }
        }

        protected override void OnRecover(object persistedEvent)
        {
            if (persistedEvent is IEvent evnt)
            {
                _state = _state.Update(evnt);
            }
        }

        protected virtual void OnPersist(IEvent persistedEvent)
        {
            _state = _state.Update(persistedEvent);
            Sender.Tell(persistedEvent, Self);
        }
    }
}
