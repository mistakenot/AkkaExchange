using Akka.Streams;
using Akka.Streams.Dsl;
using System;
using AkkaExchange.Utils;

namespace AkkaExchange.Shared.Extensions
{
    public static class SourceExtensions
    {
        public static IObservable<T> RunAsObservable<T>(
            this Source<T, Akka.NotUsed> source, 
            IMaterializer materializer)
        {
            var publisher = source.ToMaterialized(
                    Sink.Publisher<T>(),
                    Keep.Right)
                .Run(materializer);
            
            return new PublisherObservable<T>(publisher);
        }

        public static (IObservable<TOut>, TMat) RunAsObservable<TOut, TMat>(
            this Source<TOut, TMat> source, 
            IMaterializer materializer)
        {
            var (s, publisher) = source.ToMaterialized(
                    Sink.Publisher<TOut>(),
                    Keep.Both)
                .Run(materializer);
            
            return (new PublisherObservable<TOut>(publisher), s);
        }
    }
}