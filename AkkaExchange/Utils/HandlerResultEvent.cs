using System;

namespace AkkaExchange.Utils
{
    public class HandlerResultEvent
    {
        public HandlerResult Result { get; }

        public HandlerResultEvent(HandlerResult result)
        {
            Result = result ?? throw new ArgumentNullException(nameof(result));
        }
    }
}