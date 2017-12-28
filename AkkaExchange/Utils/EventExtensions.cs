using Newtonsoft.Json;

namespace AkkaExchange.Utils
{
    public static class EventExtensions
    {
        public static string Format(this IEvent value)
        {
            return $"{value.GetType().Name}: {JsonConvert.SerializeObject(value)}";
        }
    }
}
