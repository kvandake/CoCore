using System;
using UIKit;
using Foundation;
using System.Collections.Specialized;
using System.Threading;

namespace CoCore.iOS
{
	public class ObservableTableDataProvider<T>
	{
		readonly Thread _mainThread;
		readonly UITableView _tableView;
		readonly IObservableTableData<T> _source;

		public UITableView TableView {
			get {
				return _tableView;
			}
		}

		public ObservableTableDataProvider (UITableView tableView, IObservableTableData<T> source)
		{
			_mainThread = Thread.CurrentThread;
			_tableView = tableView;
			_source = source;
		}



		protected static NSIndexPath[] CreateNSIndexPathArray(int startingPosition, int count)
		{
			var newIndexPaths = new NSIndexPath[count];
			for (var i = 0; i < count; i++)
			{
				newIndexPaths[i] = NSIndexPath.FromRowSection(i + startingPosition, 0);
			}
			return newIndexPaths;
		}






		/// <summary>
		/// Collections the changed on collection changed.
		/// </summary>
		/// <param name="args">Arguments.</param>
		protected virtual void CollectionChangedCheck(NotifyCollectionChangedEventArgs args){
			if (!_source.UseAnimations) {
				ReloadTableData ();
				return;
			}
			if (TryDoAnimatedChange (args)) {
				return;
			}
			ReloadTableData ();
		}


		/// <summary>
		/// Do animated change.
		/// </summary>
		/// <returns><c>true</c>, if do animated change was tryed, <c>false</c> otherwise.</returns>
		/// <param name="e">E.</param>
		protected bool TryDoAnimatedChange(NotifyCollectionChangedEventArgs e){
			switch (e.Action) {
			case NotifyCollectionChangedAction.Add:
				var newIndexPaths = CreateNSIndexPathArray (e.NewStartingIndex, e.NewItems.Count);
				_tableView.BeginUpdates ();
				_tableView.InsertRows (newIndexPaths, _source.AddAnimation);
				_tableView.EndUpdates ();
				return true;
			case NotifyCollectionChangedAction.Remove:
				var oldIndexPaths = CreateNSIndexPathArray (e.OldStartingIndex, e.OldItems.Count);
				_tableView.BeginUpdates ();
				_tableView.DeleteRows (oldIndexPaths, _source.DeleteAnimation);
				_tableView.EndUpdates ();
				return true;
			case NotifyCollectionChangedAction.Replace:
				if (e.NewItems.Count != e.OldItems.Count)
					return false;
				var indexPath = NSIndexPath.FromRowSection (e.NewStartingIndex, 0);
				_tableView.ReloadRows (new[] {
					indexPath
				}, _source.ReplaceAnimation);
				return true;
			case NotifyCollectionChangedAction.Move:
				if (e.NewItems.Count != 1 && e.OldItems.Count != 1)
					return false;
				if (e.NewStartingIndex == e.OldStartingIndex)
					return true;
				var newPath = NSIndexPath.FromRowSection (e.NewStartingIndex, 0);
				var oldPath = NSIndexPath.FromRowSection (e.OldStartingIndex, 0);
				_tableView.BeginUpdates ();
				_tableView.MoveRow (oldPath, newPath);
				_tableView.EndUpdates ();
				return true;
			case NotifyCollectionChangedAction.Reset:
				_tableView.ReloadData ();
				return true;
			default:
				return false;
			}
		}

		public void ReloadTableData(){
			_tableView.ReloadData ();
		}


		public void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			var isMainThread = Thread.CurrentThread == _mainThread;
			if (isMainThread) {
				CollectionChangedCheck (e);
			} else {
				_tableView.InvokeOnMainThread (() => CollectionChangedCheck (e));
			}
		}

	}
}

