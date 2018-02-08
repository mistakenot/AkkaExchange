using AkkaExchange.Utils;
using System.Linq;

namespace AkkaExchange.Shared.Events
{
    public class HandlerErrorEvent
    {
        public string Name { get; }
        public string Type { get; }
        public HandlerResult Result { get; }

        public HandlerErrorEvent(string name, string type, HandlerResult result)
        {
            Name = name ?? throw new System.ArgumentNullException(nameof(name));
            Type = type ?? throw new System.ArgumentNullException(nameof(type));
            Result = result ?? throw new System.ArgumentNullException(nameof(result));
        }

        public override string ToString() =>
            $"Handler error. Type: {Type} name: {Name} msg: {Result.Errors.First()}]";
    }
}