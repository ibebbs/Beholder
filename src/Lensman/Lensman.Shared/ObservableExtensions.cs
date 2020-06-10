using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Lensman
{
    public static class ObservableExtensions
    {
        public static IObservable<TSource> UsingAsync<TSource, TResource>(Func<TResource> resourceFactory, Func<TResource, IObservable<TSource>> observableFactory)
            where TResource : IAsyncDisposable
        {
            return Observable.Create<TSource>(
                observer =>
                {
                    var resource = resourceFactory();

                    var observable = observableFactory(resource);

                    Func<ValueTask> disposeAsync = async () =>
                    {
                        if (!EqualityComparer<TResource>.Default.Equals(resource, default))
                        {
                            await resource.DisposeAsync();
                            resource = default;
                        }
                    };

                    Action<TSource> onNext = source =>
                    {
                        try
                        {
                            observer.OnNext(source);
                        }
                        catch (Exception)
                        {
                            _ = disposeAsync();

                            throw;
                        }
                    };

                    Action<Exception> onError = async exception =>
                    {
                        await disposeAsync();
                        observer.OnError(exception);
                    };

                    Action onCompleted = async () =>
                    {
                        await disposeAsync();
                        observer.OnCompleted();
                    };

                    return new CompositeDisposable(
                        observable.Subscribe(onNext, onError, onCompleted),
                        Disposable.Create(() => disposeAsync())
                    );
                });
        }
    }
}
