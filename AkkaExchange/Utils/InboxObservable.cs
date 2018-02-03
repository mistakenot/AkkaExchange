using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;

namespace AkkaExchange.Utils
{
    public class InboxObservable : IObservable<object>
    {
        private readonly Inbox _inbox;

        public InboxObservable(Inbox inbox)
        {
            _inbox = inbox;
        }

        public IDisposable Subscribe(IObserver<object> observer) 
            => new Subscription(_inbox, observer);

        class Subscription : IDisposable
        {
            private readonly CancellationTokenSource _tokenSource;
            private readonly Task _task;

            public Subscription(IInboxable inbox, IObserver<object> observer)
            {
                _tokenSource = new CancellationTokenSource();

                _task = Task.Run(async () =>
                    {
                        while (!_tokenSource.IsCancellationRequested)
                        {
                            var msg = await inbox.ReceiveAsync();
                            if (msg is Status.Failure failure && failure.Cause is TimeoutException)
                            {
                                break;
                            }

                            observer.OnNext(msg);
                        }
                    },
                    _tokenSource.Token)
                    .ContinueWith(t => observer.OnCompleted(), TaskContinuationOptions.OnlyOnRanToCompletion)
                    .ContinueWith(t => observer.OnError(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
            }

            public void Dispose()
            {
                _tokenSource.Cancel();
            }
        }
    }
}