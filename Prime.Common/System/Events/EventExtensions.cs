using System;
using System.Collections.Generic;
using System.Reactive;

namespace Prime.Common
{
    public static class EventExtensions {

        public static IDisposable Subscribe<T>(this IObservable<EventPattern<T>> observable, Action<T> action)
        {
            return observable.Subscribe(pattern => action(pattern.EventArgs));
        }

        public static void SubscribePermanent<T>(this IObservable<EventPattern<T>> observable, Action<T> action)
        {
            EventsCache.Add(observable.Subscribe(pattern => action(pattern.EventArgs)));
        }

        internal static List<IDisposable> EventsCache = new List<IDisposable>();
    }
}