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
		public virtual void AddRange(IList<T> items)
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

		public virtual void InsertWithSort(T item, Comparison<T> comparison){
			int i = 0;
			while (i < Items.Count && comparison (Items [i], item) < 0) {
				i++;
			}
			Insert (i, item);
		}

		public virtual void InsertWithSort(T item, IComparer<T> comparer){
			int i = 0;
			while (i < Items.Count && comparer.Compare (Items [i], item) < 0) {
				i++;
			}
			Insert (i, item);
		}

        /// <summary>
        /// Remove the where.
        /// </summary>
        /// <param name="match">Match.</param>
		public virtual void RemoveWhere(Predicate<T> match)
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
		public virtual void Reload()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }


        #region Update

		public virtual void UpdateWithSort(T update, Comparison<T> comparison){
			var oldPosition = IndexOf (update);
			int newPosition = -1;
			var lastPosition = Items.Count - 1;
			do {
				if (newPosition == lastPosition)
					break;
				newPosition++;
				if (oldPosition != lastPosition && newPosition == oldPosition) {
					newPosition++;
				}
			} while (newPosition < Items.Count && comparison (Items [newPosition], update) < 0);				
			Update (oldPosition);
			if (oldPosition != newPosition) {
				Move (oldPosition, newPosition);
			}
		}

		public virtual void UpdateWithSort(T item, IComparer<T> comparer){
			var oldPosition = IndexOf (item);
			int newPosition = -1;
			var lastPosition = Items.Count - 1;
			do {
				if (newPosition == lastPosition)
					break;
				newPosition++;
				if (oldPosition != lastPosition && newPosition == oldPosition) {
					newPosition++;
				}
			} while (newPosition < Items.Count && comparer.Compare (Items [newPosition], item) < 0);
			Update (oldPosition);
			if (oldPosition != newPosition) {
				Move (oldPosition, newPosition);
			}
		}

        /// <summary>
        /// Update the specified item.
        /// </summary>
        /// <param name="item">Item.</param>
		public virtual void Update(T item)
        {
            var index = IndexOf(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                new[] { item }, new[] { item }, index));
        }



        /// <summary>
        /// Update the specified index.
        /// </summary>
        /// <param name="index">Index.</param>
		public virtual void Update(int index)
        {
            var item = this[index];
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                new[] { item }, new[] { item }, index));
        }



        /// <summary>
        /// Updates the notify.
        /// </summary>
        /// <param name="match">Match.</param>
		public virtual void UpdateNotify(Predicate<T> match)
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
		public virtual void Update(Func<T, bool> action)
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

		public virtual void Sort<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer)
        {
            InternalSort(Items.OrderBy(keySelector, comparer));
        }

		public virtual void Sort<TKey>(Func<T, TKey> keySelector, Comparison<TKey> comparison)
        {
            InternalSort(Items.OrderBy(keySelector, new ComparisonComparer<TKey>(comparison)));
        }

		public virtual void Sort(Comparison<T> comparison)
        {
            InternalSort(Items.OrderBy(comparison));
        }


		protected virtual void InternalSort(IEnumerable<T> sortedItems)
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
