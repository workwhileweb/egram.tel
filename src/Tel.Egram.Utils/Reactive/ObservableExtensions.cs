using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Tel.Egram.Utils.Reactive
{
    public static class ObservableExtensions
    {
        /// <summary>
        /// Aggregate observable into list
        /// </summary>
        public static IObservable<IList<T>> CollectToList<T>(this IObservable<T> observable)
        {
            return observable.Aggregate(new List<T>(), (list, item) =>
            {
                list.Add(item);
                return list;
            });
        }
        
        /// <summary>
        /// Like SelectMany but ordered
        /// </summary>
        public static IObservable<TResult> SelectSeq<T, TResult>(
            this IObservable<T> observable,
            Func<T, IObservable<TResult>> selector)
        {
            return observable.Select(selector).Concat();
        }
    }
}