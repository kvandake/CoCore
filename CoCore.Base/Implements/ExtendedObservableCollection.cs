using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace CoCore.Base
{
    public class ExtendedObservableCollection<T> : ObservableCollection<T>
    {

        public ExtendedObservableCollection()
        {
        }

        public ExtendedObservableCollection(IEnumerable<T> items) : base(items)
        {
        }

        /// <summary>
        /// Add the range.
        /// </summary>
        /// <param name="items">Items.</param>
        public void AddRange(IList<T> items)
        {
            if (items == null)
                return;
            var startIndex = Items.Count;
            foreach (var item in items)
            {
                Items.Add(item);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)items, startIndex));
        }



        /// <summary>
        /// Remove the where.
        /// </summary>
        /// <param name="match">Match.</param>
        public void RemoveWhere(Predicate<T> match)
        {
            foreach (var item in Items.ToList())
            {
                if (match(item))
                {
                    Remove(item);
                }
            }
        }


        /// <summary>
        /// Reload this instance.
        /// </summary>
        public void Reload()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }


        #region Update


        /// <summary>
        /// Update the specified item.
        /// </summary>
        /// <param name="item">Item.</param>
        public void Update(T item)
        {
            var index = IndexOf(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                new[] { item }, new[] { item }, index));
        }



        /// <summary>
        /// Update the specified index.
        /// </summary>
        /// <param name="index">Index.</param>
        public void Update(int index)
        {
            var item = this[index];
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                new[] { item }, new[] { item }, index));
        }



        /// <summary>
        /// Updates the notify.
        /// </summary>
        /// <param name="match">Match.</param>
        public void UpdateNotify(Predicate<T> match)
        {
            foreach (var item in Items)
            {
                if (match(item))
                {
                    var index = IndexOf(item);
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                        new[] { item }, new[] { item }, index));
                }
            }
        }


        /// <summary>
        /// Update the specified action (if true then Replace Notify)
        /// </summary>
        /// <param name="action">Action.</param>
        public void Update(Func<T, bool> action)
        {
            foreach (var item in Items)
            {
                if (action(item))
                {
                    var index = IndexOf(item);
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                        new[] { item }, new[] { item }, index));
                }
            }
        }

        #endregion


        #region Sorting 

        public void Sort<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer)
        {
            InternalSort(Items.OrderBy(keySelector, comparer));
        }

        public void Sort<TKey>(Func<T, TKey> keySelector, Comparison<TKey> comparison)
        {
            InternalSort(Items.OrderBy(keySelector, new ComparisonComparer<TKey>(comparison)));
        }

        public void Sort(Comparison<T> comparison)
        {
            InternalSort(Items.OrderBy(comparison));
        }


        void InternalSort(IEnumerable<T> sortedItems)
        {
            var sortedItemsList = sortedItems.ToList();
            foreach (var item in sortedItemsList)
            {
                Move(IndexOf(item), sortedItemsList.IndexOf(item));
            }
        }

        #endregion

    }

    
}
