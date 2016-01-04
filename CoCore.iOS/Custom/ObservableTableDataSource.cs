using System;
using UIKit;
using System.Collections.Generic;
using System.Collections.Specialized;
using Foundation;

namespace CoCore.iOS
{
	public class ObservableTableDataSource<T> : UITableViewDataSource, IObservableTableData<T>
	{
		IList<T> _dataSource;
		readonly ObservableTableDataProvider<T> provider;
		INotifyCollectionChanged _notifier;

		#region IObservableTableData implementation

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
					_notifier.CollectionChanged -= provider.HandleCollectionChanged;
				}

				_dataSource = value;
				_notifier = value as INotifyCollectionChanged;

				if (_notifier != null) {
					_notifier.CollectionChanged += provider.HandleCollectionChanged;
				}
				provider.ReloadTableData ();
			}
		}

		public UITableView TableView {
			get {
				return provider.TableView;
			}
		}

		public bool UseAnimations { get; set;}

		public bool DeselectRowAfterSelect { get; set; }

		public UITableViewRowAnimation AddAnimation {get;set;}

		public UITableViewRowAnimation DeleteAnimation {get;set;}

		public UITableViewRowAnimation ReplaceAnimation {get;set;}

		public Func<UITableView, NSIndexPath, T, UITableViewCell> BindCellDelegate {get;set;}

		#endregion

		public ObservableTableDataSource (UITableView tableView)
		{
			provider = new ObservableTableDataProvider<T> (tableView, this);
			Initialize ();
		}

		void Initialize()
		{
			AddAnimation = UITableViewRowAnimation.Automatic;
			DeleteAnimation = UITableViewRowAnimation.Automatic;
			ReplaceAnimation = UITableViewRowAnimation.Automatic;
			UseAnimations = true;
			DeselectRowAfterSelect = true;

		}

		#region implemented abstract members of UITableViewDataSource

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var item = DataSource [indexPath.Row];
			var bind = BindCellDelegate;
			return bind != null ? bind (tableView, indexPath, item) : null;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return DataSource != null ? DataSource.Count : 0;
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		#endregion
	}
}

