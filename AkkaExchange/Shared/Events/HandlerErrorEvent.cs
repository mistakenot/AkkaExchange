using AkkaExchange.Utils;

namespace AkkaExchange.Shared.Events
{
    public class HandlerErrorEvent
    {
        public string Name { get; }
        public HandlerResult Result { get; }

        public HandlerErrorEvent(string name, HandlerResult result)
        {
            Name = name ?? throw new System.ArgumentNullException(nameof(name));
            Result = result ?? throw new System.ArgumentNullException(nameof(result));
        }
    }
}