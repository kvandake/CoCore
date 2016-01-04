using System;
using UIKit;
using System.Collections.Generic;
using System.Collections.Specialized;
using Foundation;
using CoreGraphics;

namespace CoCore.iOS
{
	public class ObservableTableSource<T> : UITableViewSource, IObservableTableData<T>
	{
		IList<T> _dataSource;
		readonly ObservableTableDataProvider<T> provider;
		INotifyCollectionChanged _notifier;
		T _selectedItem;

		#region Init Events

		public event EventHandler OnDraggingStarted;

		public event EventHandler OnDraggingEnded;

		public event EventHandler OnWillEndDragging;

		public event EventHandler OnScrolled;

		public event EventHandler OnScrolledToTop;

		public event EventHandler OnScrollAnimationEnded;

		public event EventHandler OnZoomingStarted;

		public event EventHandler OnZoomingEnded;

		public event EventHandler OnDidZoom;

		public event EventHandler OnDecelerationStarted;

		public event EventHandler OnDecelerationEnded;

		#endregion

	    public T SelectedItem {
			get {
				return _selectedItem;
			}
		}

		public UITableView TableView {
			get {
				return provider.TableView;
			}
		}

		public event EventHandler<ObservableEventArgs<T>> SelectionChanged;


		public bool UseAnimations { get; set;}

		public bool DeselectRowAfterSelect { get; set; }

	    /// <summary>
		/// When set, specifies which animation should be used when rows change.
		/// </summary>
		public UITableViewRowAnimation AddAnimation{get;set;}

		/// <summary>
		/// When set, specifieds which animation should be used when a row is deleted.
		/// </summary>
		public UITableViewRowAnimation DeleteAnimation{get;set;}

		/// <summary>
		/// When set, specifieds which animation should be used when a row is replaced.
		/// </summary>
		public UITableViewRowAnimation ReplaceAnimation{get;set;}

		public Func<UITableView, NSIndexPath,T, UITableViewCell> BindCellDelegate{get;set;}

		/// <summary>
		/// When set, returns the height of the view that will be used for the TableView's footer.
		/// </summary>
		/// <seealso cref="GetViewForFooterDelegate"/>
		public Func<UITableView, nint, nfloat> GetHeightForFooterDelegate{get;set;}

		/// <summary>
		/// When set, returns the height of the view that will be used for the TableView's header.
		/// </summary>
		/// <seealso cref="GetViewForHeaderDelegate"/>
		public Func<UITableView, nint, nfloat> GetHeightForHeaderDelegate{get;set;}

		/// <summary>
		/// When set, returns the height of rows.
		/// </summary>
		/// <seealso cref="GetViewForHeaderDelegate"/>
		public Func<UITableView, NSIndexPath, nfloat> GetHeightForRowDelegate {get;set;}

		/// <summary>
		/// When set, returns a view that can be used as the TableView's footer.
		/// </summary>
		/// <seealso cref="GetHeightForFooterDelegate"/>
		public Func<UITableView, nint, UIView> GetViewForFooterDelegate{get;set;}

		/// <summary>
		/// When set, returns a view that can be used as the TableView's header.
		/// </summary>
		/// <seealso cref="GetHeightForHeaderDelegate"/>
		public Func<UITableView, nint, UIView> GetViewForHeaderDelegate{get;set;}


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


		public ObservableTableSource(UITableView tableView){
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

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return GetHeightForRowDelegate != null ? GetHeightForRowDelegate (tableView, indexPath) : tableView.RowHeight;
		}


		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var item = DataSource [indexPath.Row];
			var bind = BindCellDelegate;
			return bind != null ? bind (tableView, indexPath, item) : null;
		}








		#region Implements events

		public override void DraggingEnded (UIScrollView scrollView, bool willDecelerate)
		{
			RaiseDraggingEnded (EventArgs.Empty);
		}

		public override void DraggingStarted (UIScrollView scrollView)
		{
			RaiseDraggingStarted (EventArgs.Empty);
		}

		public override void WillEndDragging (UIScrollView scrollView, CGPoint velocity, ref CGPoint targetContentOffset)
		{
			RaiseWillEndDragging (EventArgs.Empty);
		}

		public override void Scrolled (UIScrollView scrollView)
		{
			RaiseScrolled (EventArgs.Empty);
		}

		public override void ScrolledToTop (UIScrollView scrollView)
		{
			RaiseScrolledToTop (EventArgs.Empty);
		}
			
		public override void ScrollAnimationEnded (UIScrollView scrollView)
		{
			RaiseScrollAnimationEnded (EventArgs.Empty);
		}

		public override void ZoomingStarted (UIScrollView scrollView, UIView view)
		{
			RaiseZoomingStarted (EventArgs.Empty);
		}

		public override void ZoomingEnded (UIScrollView scrollView, UIView withView, nfloat atScale)
		{
			RaiseZoomingEnded (EventArgs.Empty);
		}

		public override void DidZoom (UIScrollView scrollView)
		{
			RaiseDidZoom (EventArgs.Empty);
		}

		public override void DecelerationStarted (UIScrollView scrollView)
		{
			RaiseDecelerationStarted (EventArgs.Empty);
		}

		public override void DecelerationEnded (UIScrollView scrollView)
		{
			RaiseDecelerationEnded (EventArgs.Empty);
		}

		#endregion

		#region Events

		protected virtual void RaiseDraggingStarted (EventArgs e)
		{
			var handler = OnDraggingStarted;
			if (handler != null)
				handler (this, e);
		}


		protected virtual void RaiseDraggingEnded (EventArgs e)
		{
			var handler = OnDraggingEnded;
			if (handler != null)
				handler (this, e);
		}

		protected virtual void RaiseWillEndDragging (EventArgs e)
		{
			var handler = OnWillEndDragging;
			if (handler != null)
				handler (this, e);
		}

		protected virtual void RaiseScrolled (EventArgs e)
		{
			var handler = OnScrolled;
			if (handler != null)
				handler (this, e);
		}

		protected virtual void RaiseScrolledToTop (EventArgs e)
		{
			var handler = OnScrolledToTop;
			if (handler != null)
				handler (this, e);
		}


		protected virtual void RaiseScrollAnimationEnded (EventArgs e)
		{
			var handler = OnScrollAnimationEnded;
			if (handler != null)
				handler (this, e);
		}

		protected virtual void RaiseZoomingStarted (EventArgs e)
		{
			var handler = OnZoomingStarted;
			if (handler != null)
				handler (this, e);
		}



		protected virtual void RaiseZoomingEnded (EventArgs e)
		{
			var handler = OnZoomingEnded;
			if (handler != null)
				handler (this, e);
		}

		protected virtual void RaiseDidZoom (EventArgs e)
		{
			var handler = OnDidZoom;
			if (handler != null)
				handler (this, e);
		}

		protected virtual void RaiseDecelerationStarted (EventArgs e)
		{
			var handler = OnDecelerationStarted;
			if (handler != null)
				handler (this, e);
		}

		protected virtual void RaiseDecelerationEnded (EventArgs e)
		{
			var handler = OnDecelerationEnded;
			if (handler != null)
				handler (this, e);
		}
		#endregion
	}
}

