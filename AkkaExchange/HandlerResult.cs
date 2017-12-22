using System.Collections.Generic;
using System.Linq;

namespace AkkaExchange
{
    public class HandlerResult
    {
        public bool Success { get; }
        public IEnumerable<string> Errors { get; }
        public IEvent Event { get; }

        public HandlerResult(string error)
            : this(new []{error})
        {
            
        }

        public HandlerResult(IEnumerable<string> errors)
        {
            Success = false;
            Errors = errors;
            Event = null;
        }

        public HandlerResult(IEvent evnt)
            : this(Enumerable.Empty<string>())
        {
            Event = evnt;
            Success = true;
        }

        public static HandlerResult NotFound(object command) => 
            new HandlerResult(
                $"Event type {command.GetType()} not supported by handler.");
    }
}
