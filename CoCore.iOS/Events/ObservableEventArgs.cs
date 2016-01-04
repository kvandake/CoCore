using System;
using Foundation;

namespace CoCore.iOS
{
	public class ObservableEventArgs<T> : EventArgs{

		readonly T _item;

		public T Item {
			get {
				return _item;
			}
		}

		readonly NSIndexPath _index;

		public NSIndexPath Index {
			get {
				return _index;
			}
		}

		public ObservableEventArgs(NSIndexPath index, T item){
			_index = index;
			_item = item; 

		}
	}
}

