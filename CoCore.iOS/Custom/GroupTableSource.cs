using System;
using UIKit;
using Foundation;
using System.Collections.Specialized;
using System.Threading;
using CoCore.Base;

namespace CoCore.iOS
{
	public class GroupTableSource : UITableViewSource
	{
		readonly Thread _mainThread;
		GroupRoot _dataSource;
		readonly UITableView _tableView;


	    public event EventHandler<NSIndexPath> SelectionChanged;

        public GroupCell SelectedItem { get; private set; }

		public bool UseAnimations { get; set;}

	    public UITableView TableView {
			get {
				return _tableView;
			}
		}

	    public UITableViewRowAnimation AddSectionAnimation
		{
			get;
			set;
		}


		public UITableViewRowAnimation DeleteSectionAnimation
		{
			get;
			set;
		}

		public UITableViewRowAnimation AddCellAnimation
		{
			get;
			set;
		}


		public UITableViewRowAnimation DeleteCellAnimation
		{
			get;
			set;
		}

        public UITableViewRowAnimation ReloadSectionAnimation
        {
            get;
            set;
        }

        public UITableViewRowAnimation ReplaceCellAnimation
        {
            get;
            set;
        }




        public GroupRoot DataSource {
			get {
				return _dataSource;
			}
			set {
				if (Equals (_dataSource, value)) {
					return;
				}
				if (_dataSource != null) {
					_dataSource.CollectionChanged -= HandleSectionCollectionChanged;
					foreach (var section in _dataSource) {
						section.CollectionChanged -= HandleCellCollectionChanged;
					}
				}
				_dataSource = value;
				_dataSource.CollectionChanged += HandleSectionCollectionChanged;
				foreach (var section in value) {
					section.CollectionChanged += HandleCellCollectionChanged;
				}
				_tableView.ReloadData ();
			}
		}


		public GroupTableSource (UITableView tableView)
		{
			_tableView = tableView;
			_mainThread = Thread.CurrentThread;
			UseAnimations = true;
			AddCellAnimation = UITableViewRowAnimation.Automatic;
			DeleteCellAnimation = UITableViewRowAnimation.Automatic;
			AddSectionAnimation = UITableViewRowAnimation.Automatic;
			DeleteSectionAnimation = UITableViewRowAnimation.Automatic;
            ReplaceCellAnimation = UITableViewRowAnimation.Automatic;
            ReloadSectionAnimation = UITableViewRowAnimation.Automatic;
        }

		public GroupTableSource (UITableView tableView, GroupRoot dataSource):this(tableView)
		{
			DataSource = dataSource;
		}


		public override nint NumberOfSections (UITableView tableView)
		{
			return _dataSource == null ? 0 : _dataSource.Count;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return _dataSource == null ? 0 : _dataSource [(int)section].Count;
		}

		public Func<UITableView,GroupCell,NSIndexPath,UITableViewCell> CreateCell{ get; set;}


		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var itemData = _dataSource [indexPath.Section] [indexPath.Row];
			if (itemData == null)
				return null;
			var isCustom = itemData.CellStyle.Has (GroupCellStyle.Custom);
			if (isCustom) {
				return CreateCell != null ? CreateCell (tableView, itemData, indexPath) : null;
			}
		    var isNone = itemData.CellStyle.Has (GroupCellStyle.None);
		    var isPrimary = itemData.CellStyle.Has (GroupCellStyle.Primary);
		    var isSecondary = itemData.CellStyle.Has (GroupCellStyle.Secondary);
		    var isDetail = itemData.CellStyle.Has (GroupCellStyle.Detail);
		    var isCheckMark = itemData.CellStyle.Has (GroupCellStyle.CheckMark);
		    var isDisclosure = itemData.CellStyle.Has (GroupCellStyle.Disclosure);
		    UITableViewCell cell = null;
		    var ident = itemData.Tag ?? itemData.GetHashCode ().ToString ();
		    if (isPrimary) {
		        cell = tableView.DequeueReusableCell (ident) ?? new UITableViewCell (UITableViewCellStyle.Default, ident);
		        cell.TextLabel.Text = itemData.PrimaryText;
		    } else if (isSecondary) {
		        cell = tableView.DequeueReusableCell (ident) ?? new UITableViewCell (UITableViewCellStyle.Subtitle, ident);
		        cell.TextLabel.Text = itemData.PrimaryText;
		        cell.DetailTextLabel.Text = itemData.SecondaryText;
		    }
		    if (cell == null) return null;
		    if (isNone)
		    {
		        cell.Accessory = UITableViewCellAccessory.None;
		    }
		    else if (isDetail)
		    {
		        cell.Accessory = UITableViewCellAccessory.DetailButton;
		    }
		    else if (isDisclosure)
		    {
		        cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
		    }
		    else if (isCheckMark)
		    {
		        cell.Accessory = itemData.IsSelected ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
		    }
		    return cell;
		}

	    public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
	    {
	        var item = _dataSource[indexPath.Section][indexPath.Row];
	        if (item == null) return;
	        var handler = SelectionChanged;
	        if (handler != null)
	        {
	            SelectedItem = item;
	            handler(this, indexPath);
	        }
	        else
	        {
	            var isRowClick = item.CellStyle.Has(GroupCellStyle.RowClick);
	            var isCheckMark = item.CellStyle.Has(GroupCellStyle.CheckMark);
	            if (isRowClick)
	            {
	                if (item.Command != null && item.Command.CanExecute(indexPath))
	                {
	                    item.Command.Execute(indexPath);
	                }
	                tableView.DeselectRow(indexPath, true);
	            }
	            if (isCheckMark)
	            {
	                item.IsSelected = !item.IsSelected;
	                tableView.ReloadRows(new[] {indexPath}, UITableViewRowAnimation.Automatic);
	            }
	        }
	    }

	    public override void AccessoryButtonTapped (UITableView tableView, NSIndexPath indexPath)
		{
			var item = _dataSource [indexPath.Section] [indexPath.Row];
			if (item != null) {
				var isDetail = item.CellStyle.Has (GroupCellStyle.Detail);
				if (isDetail) {
					if (item.AccessoryCommand != null && item.AccessoryCommand.CanExecute (indexPath)) {
						item.AccessoryCommand.Execute (indexPath);
					}
				}
			}
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return _dataSource [(int)section].Title;
		}


		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			var item = _dataSource [indexPath.Section] [indexPath.Row];
			var isCustom = item.CellStyle.Has (GroupCellStyle.Custom);
			if (isCustom) {
				var height = item.Height;
				return Math.Abs (height) <= 0 ? tableView.RowHeight : height;
			}
		    return tableView.RowHeight;
		}


	    public override nfloat GetHeightForHeader(UITableView tableView, nint section)
	    {
	        var item = _dataSource[(int) section];
	        var height = item.Height;
	        return Math.Abs(height) <= 0 ? tableView.SectionHeaderHeight : height;
	    }

	    #region Action


		#region Section Change

		void HandleSectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e){
			var isMainThread = Thread.CurrentThread == _mainThread;
			if (isMainThread) {
				SectionCollectionChangedCheck (e);
			} else {
				_tableView.InvokeOnMainThread (() => SectionCollectionChangedCheck (e));
			}
		}

		protected virtual void SectionCollectionChangedCheck(NotifyCollectionChangedEventArgs args){
			if (!UseAnimations) {
				ReloadTableData ();
				return;
			}
			if (SectionTryDoAnimatedChange (args)) {
				return;
			}
			ReloadTableData ();
		}


		protected bool SectionTryDoAnimatedChange(NotifyCollectionChangedEventArgs e){
			switch (e.Action) {
			case NotifyCollectionChangedAction.Add:
				var countAdd = e.NewItems.Count;
				for (var i = 0; i < countAdd; i++) {
					_tableView.BeginUpdates ();
					var addSection = e.NewItems [i] as GroupSection;
					if (addSection != null) {
						addSection.CollectionChanged += HandleCellCollectionChanged;
					}
					var section = NSIndexSet.FromIndex (e.NewStartingIndex + i);
					_tableView.InsertSections (section, AddSectionAnimation);
					_tableView.EndUpdates ();
				}
				return true;
			case NotifyCollectionChangedAction.Remove:
				var countRemove = e.OldItems.Count;
				for (var i = 0; i < countRemove; i++) {
					var removeSection = e.OldItems [i] as GroupSection;
					if (removeSection != null) {
						removeSection.CollectionChanged -= HandleCellCollectionChanged;
					}
					_tableView.BeginUpdates ();
					var section = NSIndexSet.FromIndex (e.OldStartingIndex + i);
					_tableView.DeleteSections (section, DeleteSectionAnimation);
					_tableView.EndUpdates ();
				}
				return true;
			case NotifyCollectionChangedAction.Replace:
				if (e.NewItems.Count != e.OldItems.Count)
					return false;
				_tableView.ReloadSectionIndexTitles ();
				var sectionReplace = NSIndexSet.FromIndex (e.NewStartingIndex);
				_tableView.ReloadSections (sectionReplace, ReloadSectionAnimation);
				return true;
			case NotifyCollectionChangedAction.Reset:
				ReloadTableData ();
				return true;
			case NotifyCollectionChangedAction.Move:
				if (e.NewItems.Count != 1 && e.OldItems.Count != 1)
					return false;
				if (e.NewStartingIndex == e.OldStartingIndex)
					return true;
				_tableView.BeginUpdates ();
				_tableView.MoveSection (e.OldStartingIndex, e.NewStartingIndex);
				_tableView.EndUpdates ();
				return true;
			default:
				return false;
			}
		}

		#endregion

		#region Cell Change

		void HandleCellCollectionChanged(object sender, NotifyCollectionChangedEventArgs e){
			var isMainThread = Thread.CurrentThread == _mainThread;
			if (isMainThread) {
				CellCollectionChangedCheck (sender,e);
			} else {
				_tableView.InvokeOnMainThread (() => CellCollectionChangedCheck (sender, e));
			}
		}

		/// <summary>
		/// Collections the changed on collection changed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		protected virtual void CellCollectionChangedCheck(object sender, NotifyCollectionChangedEventArgs args){
			if (!UseAnimations) {
				ReloadTableData ();
				return;
			}
			if (CellTryDoAnimatedChange (sender, args)) {
				return;
			}
			ReloadTableData ();
		}

		protected bool CellTryDoAnimatedChange(object sender, NotifyCollectionChangedEventArgs e){
			var section = sender as GroupSection;
			if (section == null)
				return false;
			var indexSection = DataSource.IndexOf (section);
			switch (e.Action) {
			case NotifyCollectionChangedAction.Add:
				var countAdd = e.NewItems.Count;
				var pathsAdd = new NSIndexPath[countAdd];
				for (var i = 0; i < countAdd; i++) {
					pathsAdd [i] = NSIndexPath.FromRowSection (e.NewStartingIndex + i, indexSection);
				}
				_tableView.BeginUpdates ();
				_tableView.InsertRows (pathsAdd, AddCellAnimation);
				_tableView.EndUpdates ();
				return true;
			case NotifyCollectionChangedAction.Remove:
				var countRemove = e.OldItems.Count;
				var pathsRemove = new NSIndexPath[countRemove];
				for (var i = 0; i < countRemove; i++) {
					pathsRemove [i] = NSIndexPath.FromRowSection (e.OldStartingIndex + i, indexSection);
				}
				_tableView.BeginUpdates ();
				_tableView.DeleteRows (pathsRemove, DeleteCellAnimation);
				_tableView.EndUpdates ();
				return true;
			case NotifyCollectionChangedAction.Replace:
				if (e.NewItems.Count != e.OldItems.Count)
					return false;
				var indexPath = NSIndexPath.FromRowSection (e.NewStartingIndex, indexSection);
				_tableView.ReloadRows (new[] {
					indexPath
				}, ReplaceCellAnimation);
				return true;
			case NotifyCollectionChangedAction.Move:
				if (e.NewItems.Count != 1 && e.OldItems.Count != 1)
					return false;
				if (e.NewStartingIndex == e.OldStartingIndex)
					return true;
				var newPath = NSIndexPath.FromRowSection (e.NewStartingIndex, indexSection);
				var oldPath = NSIndexPath.FromRowSection (e.OldStartingIndex, indexSection);
				_tableView.BeginUpdates ();
				_tableView.MoveRow (oldPath, newPath);
				_tableView.EndUpdates ();
				return true;
			case NotifyCollectionChangedAction.Reset:
				ReloadTableData ();
				return true;
			default:
				return false;
			}
		}


		#endregion

		void ReloadTableData(){
			_tableView.ReloadData ();
		}

		#endregion

	    protected virtual void OnSelectionChanged(NSIndexPath e)
	    {
	        SelectionChanged?.Invoke(this, e);
	    }
	}
}

