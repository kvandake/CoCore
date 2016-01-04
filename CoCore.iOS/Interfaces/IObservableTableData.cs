using System;
using UIKit;
using Foundation;
using System.Collections.Generic;

namespace CoCore.iOS
{
	public interface IObservableTableData<T>
	{

		IList<T> DataSource {get;set;}

		UITableView TableView {get;}

		bool UseAnimations { get; set;}

		bool DeselectRowAfterSelect { get; set; }

		/// <summary>
		/// When set, specifies which animation should be used when rows change.
		/// </summary>
		UITableViewRowAnimation AddAnimation{get;set;}

		/// <summary>
		/// When set, specifieds which animation should be used when a row is deleted.
		/// </summary>
		UITableViewRowAnimation DeleteAnimation{get;set;}

		/// <summary>
		/// When set, specifieds which animation should be used when a row is replaced.
		/// </summary>
		UITableViewRowAnimation ReplaceAnimation{get;set;}

		Func<UITableView, NSIndexPath,T, UITableViewCell> BindCellDelegate{get;set;}

	}
}

