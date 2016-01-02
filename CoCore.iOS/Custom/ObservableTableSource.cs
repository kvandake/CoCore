using System;
using UIKit;
using System.Collections.Generic;
using System.Collections.Specialized;
using Foundation;
using System.Threading;

namespace CoCore.iOS
{
	public class ObservableTableSource<T> : UITableViewSource
	{
		IList<T> _dataSource;
		Thread _mainThread;
		INotifyCollectionChanged _notifier;
		readonly UITableView _tableView;
		T _selectedItem;
		bool deselectRowAfterSelect = true;

		public T SelectedItem {
			get {
				return _selectedItem;
			}
		}

		public UITableView TableView {
			get {
				return _tableView;
			}
		}

		public event EventHandler<ObservableEventArgs<T>> SelectionChanged;


		public bool DeselectRowAfterSelect {
			get {
				return deselectRowAfterSelect;
			}
			set {
				deselectRowAfterSelect = value;
			}
		}

		/// <summary>
		/// When set, specifies which animation should be used when rows change.
		/// </summary>
		public UITableViewRowAnimation AddAnimation
		{
			get;
			set;
		}

		/// <summary>
		/// When set, specifieds which animation should be used when a row is deleted.
		/// </summary>
		public UITableViewRowAnimation DeleteAnimation
		{
			get;
			set;
		}

		public Func<UITableView, NSIndexPath,T, UITableViewCell> BindCellDelegate
		{
			get;
			set;
		}

		/// <summary>
		/// When set, returns the height of the view that will be used for the TableView's footer.
		/// </summary>
		/// <seealso cref="GetViewForFooterDelegate"/>
		public Func<UITableView, nint, nfloat> GetHeightForFooterDelegate
		{
			get;
			set;
		}

		/// <summary>
		/// When set, returns the height of the view that will be used for the TableView's header.
		/// </summary>
		/// <seealso cref="GetViewForHeaderDelegate"/>
		public Func<UITableView, nint, nfloat> GetHeightForHeaderDelegate
		{
			get;
			set;
		}

		/// <summary>
		/// When set, returns a view that can be used as the TableView's footer.
		/// </summary>
		/// <seealso cref="GetHeightForFooterDelegate"/>
		public Func<UITableView, nint, UIView> GetViewForFooterDelegate
		{
			get;
			set;
		}

		/// <summary>
		/// When set, returns a view that can be used as the TableView's header.
		/// </summary>
		/// <seealso cref="GetHeightForHeaderDelegate"/>
		public Func<UITableView, nint, UIView> GetViewForHeaderDelegate
		{
			get;
			set;
		}


		/// <summary>
		/// The data source of this list controller.
		/// </summary>
		public IList<T> DataSource {
			get {
				return _dataSource;
			}

			set {
				if (Equals (_dataSource, value)) {
					return;
				}

				if (_notifier != null) {
					_notifier.CollectionChanged -= HandleCollectionChanged;
				}

				_dataSource = value;
				_notifier = value as INotifyCollectionChanged;

				if (_notifier != null) {
					_notifier.CollectionChanged += HandleCollectionChanged;
				}
				_tableView.ReloadData ();
			}
		}


		public ObservableTableSource(UITableView tableView){
			_tableView = tableView;
			Initialize ();
		}

		void Initialize()
		{
			_mainThread = Thread.CurrentThread;
			AddAnimation = UITableViewRowAnimation.Automatic;
			DeleteAnimation = UITableViewRowAnimation.Automatic;
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return DataSource != null ? DataSource.Count : 0;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			_selectedItem = _dataSource != null ? _dataSource [indexPath.Row] : default(T);
			var handler = SelectionChanged;
			if (handler != null) {
				handler (this, new ObservableEventArgs<T> (indexPath, _selectedItem));

			}
			if (DeselectRowAfterSelect) {
				tableView.DeselectRow (indexPath, true);
			}
		}


		public override nfloat GetHeightForFooter (UITableView tableView, nint section)
		{
			return GetHeightForFooterDelegate != null 
				? GetHeightForFooterDelegate (tableView, section) 
				: 0;
		}

		public override nfloat GetHeightForHeader (UITableView tableView, nint section)
		{
			return GetHeightForHeaderDelegate != null 
				? GetHeightForHeaderDelegate (tableView, section) 
				: 0;
		}

		public override UIView GetViewForHeader (UITableView tableView, nint section)
		{
			return GetViewForHeaderDelegate != null 
				? GetViewForHeaderDelegate (tableView, section) 
					: base.GetViewForHeader (tableView, section);
		}

		public override UIView GetViewForFooter (UITableView tableView, nint section)
		{
			return GetViewForFooterDelegate != null 
				? GetViewForFooterDelegate (tableView, section) 
					: base.GetViewForFooter (tableView, section);
		}


		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var item = DataSource [indexPath.Row];
			var bind = BindCellDelegate;
			return bind != null ? bind (tableView, indexPath, item) : null;
		}


		void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			Action act = () =>
			{
				switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					var countAdd = e.NewItems.Count;
					var pathsAdd = new NSIndexPath[countAdd];
					for (var i = 0; i < countAdd; i++)
					{
						pathsAdd[i] = NSIndexPath.FromRowSection(e.NewStartingIndex + i, 0);
					}
					_tableView.BeginUpdates();
					_tableView.InsertRows(pathsAdd, AddAnimation);
					_tableView.EndUpdates();
					break;
				case NotifyCollectionChangedAction.Remove:
					var countRemove = e.OldItems.Count;
					var pathsRemove = new NSIndexPath[countRemove];
					for (var i = 0; i < countRemove; i++)
					{
						pathsRemove[i] = NSIndexPath.FromRowSection(e.OldStartingIndex + i, 0);
					}
					_tableView.BeginUpdates();
					_tableView.DeleteRows(pathsRemove, DeleteAnimation);
					_tableView.EndUpdates();
					break;
				case NotifyCollectionChangedAction.Replace:
					if (e.NewItems.Count != e.OldItems.Count)
						return;
					var indexPath = NSIndexPath.FromRowSection(e.NewStartingIndex, 0);
					_tableView.ReloadRows(new[]
						{
							indexPath
						}, UITableViewRowAnimation.Fade);
					break;
				case NotifyCollectionChangedAction.Move:
					if (e.NewItems.Count != 1 && e.OldItems.Count != 1)
						return;
					var newPath = NSIndexPath.FromRowSection(e.NewStartingIndex, 0);
					var oldPath = NSIndexPath.FromRowSection(e.OldStartingIndex , 0);
					_tableView.BeginUpdates();
					_tableView.MoveRow(oldPath,newPath);
					_tableView.EndUpdates();
					break;
				case NotifyCollectionChangedAction.Reset:
					_tableView.ReloadData();
					break;
				default:
					throw new ArgumentOutOfRangeException ();
				}
			};

			var isMainThread = Thread.CurrentThread == _mainThread;

			if (isMainThread)
			{
				act();
			}
			else
			{
				NSOperationQueue.MainQueue.AddOperation(act);
				NSOperationQueue.MainQueue.WaitUntilAllOperationsAreFinished();
			}
		}


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
}

