using System.Collections.Generic;
using System.Linq;

namespace AkkaExchange.Utils
{
    public class HandlerResult
    {
        public bool Success { get; }
        public bool WasHandled { get; }
        public IEvent Event { get; }
        public IEnumerable<string> Errors { get; }

        public HandlerResult(string error)
            : this(new []{error})
        {
            
        }

        public HandlerResult(IEnumerable<string> errors)
        {
            Success = false;
            Errors = errors;
            Event = null;
            WasHandled = true;
        }

        public HandlerResult(IEvent evnt)
            : this(Enumerable.Empty<string>())
        {
            Event = evnt;
            Success = true;
        }

        private HandlerResult()
            : this("Command not handled.")
        {
            WasHandled = false;
        }

        public static HandlerResult NotFound(object command) => 
            new HandlerResult(
                $"Event type {command.GetType()} not supported by handler.");

        public static readonly HandlerResult NotHandled =
            new HandlerResult();
    }
}
