﻿using System;
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

	    public UITableView TableView => _tableView;

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

        public UITableViewRowAnimation ReloadCellAnimation
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
			AddCellAnimation = UITableViewRowAnimation.Automatic;
			DeleteCellAnimation = UITableViewRowAnimation.Automatic;
			AddSectionAnimation = UITableViewRowAnimation.Automatic;
			DeleteSectionAnimation = UITableViewRowAnimation.Automatic;
            ReloadCellAnimation = UITableViewRowAnimation.Automatic;
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

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			var item = _dataSource [indexPath.Section] [indexPath.Row];
			if (item != null) {
				var isRowClick = item.CellStyle.Has (GroupCellStyle.RowClick);
				var isCheckMark = item.CellStyle.Has (GroupCellStyle.CheckMark);
				if (isRowClick) {
					if (item.Command != null && item.Command.CanExecute (indexPath)) {
						item.Command.Execute (indexPath);
					}
                    tableView.DeselectRow(indexPath, true);
                }
				if (isCheckMark) {
					item.IsSelected = !item.IsSelected;
					tableView.ReloadRows (new[]{ indexPath }, UITableViewRowAnimation.Automatic);
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
				if (_dataSource == null)
					return 0;
				var height = _dataSource [indexPath.Section] [indexPath.Row].Height;
				return Math.Abs (height) <= 0 ? tableView.RowHeight : height;
			} else {
				return tableView.RowHeight;
			}
		}


		#region Action

		void HandleSectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e){
			Action act = () =>
			{
				switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					var countAdd = e.NewItems.Count;
					for (var i = 0; i < countAdd; i++)
					{
						_tableView.BeginUpdates();
						var section = NSIndexSet.FromIndex(e.NewStartingIndex + i);
						_tableView.InsertSections (section,AddSectionAnimation);
						_tableView.EndUpdates();
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					var countRemove = e.OldItems.Count;
					for (var i = 0; i < countRemove; i++)
					{
						_tableView.BeginUpdates();
						var section = NSIndexSet.FromIndex(e.OldStartingIndex + i);
						_tableView.DeleteSections (section,DeleteSectionAnimation);
						_tableView.EndUpdates();
					}
					break;
				case NotifyCollectionChangedAction.Replace:
					if (e.NewItems.Count != e.OldItems.Count)
						return;
					_tableView.ReloadSectionIndexTitles ();
					var sectionReplace = NSIndexSet.FromIndex(e.NewStartingIndex);
					_tableView.ReloadSections(sectionReplace, ReloadSectionAnimation);
					break;
				case NotifyCollectionChangedAction.Reset:
					_tableView.ReloadData();
					break;
				case NotifyCollectionChangedAction.Move:
					if (e.NewItems.Count != 1 && e.OldItems.Count != 1)
						return;
					_tableView.BeginUpdates();
					_tableView.MoveSection (e.OldStartingIndex,e.NewStartingIndex);
					_tableView.EndUpdates();
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

		void HandleCellCollectionChanged(object sender, NotifyCollectionChangedEventArgs e){
			var section = sender as GroupSection;
			if (section == null)
				return;
			var indexSection = DataSource.IndexOf (section);
			Action act = () =>
			{
				switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					var countAdd = e.NewItems.Count;
					var pathsAdd = new NSIndexPath[countAdd];
					for (var i = 0; i < countAdd; i++)
					{
						pathsAdd[i] = NSIndexPath.FromRowSection(e.NewStartingIndex + i, indexSection);
					}
					_tableView.BeginUpdates();
					_tableView.InsertRows(pathsAdd, AddCellAnimation);
					_tableView.EndUpdates();
					break;
				case NotifyCollectionChangedAction.Remove:
					var countRemove = e.OldItems.Count;
					var pathsRemove = new NSIndexPath[countRemove];
					for (var i = 0; i < countRemove; i++)
					{
						pathsRemove[i] = NSIndexPath.FromRowSection(e.OldStartingIndex + i, indexSection);
					}
					_tableView.BeginUpdates();
					_tableView.DeleteRows(pathsRemove, DeleteCellAnimation);
					_tableView.EndUpdates();
					break;
				case NotifyCollectionChangedAction.Replace:
					if (e.NewItems.Count != e.OldItems.Count)
						return;
					var indexPath = NSIndexPath.FromRowSection(e.NewStartingIndex, indexSection);
					_tableView.ReloadRows(new[]
						{
							indexPath
						}, ReloadCellAnimation);
					break;
				case NotifyCollectionChangedAction.Move:
					if (e.NewItems.Count != 1 && e.OldItems.Count != 1)
						return;
					var newPath = NSIndexPath.FromRowSection(e.NewStartingIndex, indexSection);
					var oldPath = NSIndexPath.FromRowSection(e.OldStartingIndex , indexSection);
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

		#endregion
	}
}

