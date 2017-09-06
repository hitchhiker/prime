using System;
using System.Reactive;

namespace Prime.Core
{
    public class ObservableEvent<T, T2> : IObservable<EventPattern<T>> where T : EventArgs
    {
        public ObservableEvent(Func<T2, T> createArgs)
        {
            Observable = System.Reactive.Linq.Observable.FromEventPattern<T>(ev => Handler += ev, ev => Handler -= ev);
            _createArgs = createArgs;
        }

        public void Publish(T2 value)
        {
            Handler?.Invoke(this, _createArgs(value));
        }

        public void Publish(T args)
        {
            Handler?.Invoke(this, args);
        }

        private readonly Func<T2, T> _createArgs;
        private event EventHandler<T> Handler;

        public IObservable<EventPattern<T>> Observable;

        public IDisposable Subscribe(IObserver<EventPattern<T>> observer)
        {
            return Observable.Subscribe(observer);
        }
    }

    public class ObservableEvent<T, T2, T3> : IObservable<EventPattern<T>> where T : EventArgs
    {
        public ObservableEvent(Func<T2, T3, T> createArgs)
        {
            Observable = System.Reactive.Linq.Observable.FromEventPattern<T>(ev => Handler += ev, ev => Handler -= ev);
            _createArgs = createArgs;
        }

        public void Publish(T2 value, T3 value2)
        {
            Handler?.Invoke(this, _createArgs(value, value2));
        }

        public void Publish(T args)
        {
            Handler?.Invoke(this, args);
        }

        private readonly Func<T2, T3, T> _createArgs;
        private event EventHandler<T> Handler;

        public IObservable<EventPattern<T>> Observable;

        public IDisposable Subscribe(IObserver<EventPattern<T>> observer)
        {
            return Observable.Subscribe(observer);
        }
    }

    public class ObservableEvent<T, T2, T3, T4> : IObservable<EventPattern<T>> where T : EventArgs
    {
        public ObservableEvent(Func<T2, T3, T4, T> createArgs)
        {
            Observable = System.Reactive.Linq.Observable.FromEventPattern<T>(ev => Handler += ev, ev => Handler -= ev);
            _createArgs = createArgs;
        }

        public void Publish(T2 value, T3 value2, T4 value3)
        {
            Handler?.Invoke(this, _createArgs(value, value2, value3));
        }

        public void Publish(T args)
        {
            Handler?.Invoke(this, args);
        }

        private readonly Func<T2, T3, T4, T> _createArgs;
        private event EventHandler<T> Handler;

        public IObservable<EventPattern<T>> Observable;

        public IDisposable Subscribe(IObserver<EventPattern<T>> observer)
        {
            return Observable.Subscribe(observer);
        }
    }
}