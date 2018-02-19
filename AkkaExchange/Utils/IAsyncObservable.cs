using System;
using System.Threading.Tasks;

namespace AkkaExchange.Utils
{
    // TODO dont know if this is a good solution to solve the
    // Dispose() called before OnSubscribe issue.
    public interface IAsyncObservable<T>
    {
        Task<IDisposable> Subscribe(IObserver<T> observer);
    }
}