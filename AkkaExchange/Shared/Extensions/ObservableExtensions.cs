using System;
using System.Reactive.Linq;

namespace AkkaExchange.Shared.Extensions
{
    public static class ObservableExtensions
    {
        public static IObservable<T> Match<T, S>(this IObservable<S> observable) where T : class
            => observable.Where(o => o is T).Select(o => o as T);
    }
}