using System;
using CoCore.Base;
using UIKit;

namespace CoCore.iOS
{
	public class MessagePresenter : IMessagePresenter
	{
		string _okText = @"Ok";
		string _cancelText = @"Cancel";
		public string Header { get; set;}


		public string OkText {
			get {
				return _okText;
			}
			set {
				_okText = value;
			}
		}


		public string CancelText {
			get {
				return _cancelText;
			}
			set {
				_cancelText = value;
			}
		}


		#region IMessagePresenter implementation

		public void Message (string text, string description,Action okClick, Action cancelClick, params object[] parameters)
		{
			var okAlertController = UIAlertController.Create (text, description, UIAlertControllerStyle.Alert);
			//Add Action
			okAlertController.AddAction (UIAlertAction.Create (OkText, UIAlertActionStyle.Default, alert => okClick ()));
			//Cancel Action
			if (cancelClick != null) {
				okAlertController.AddAction (UIAlertAction.Create (CancelText, UIAlertActionStyle.Default, alert => {
					if(cancelClick!=null){
					cancelClick ();
					}
				}));
			}
			var topController = NavigationService.TopViewControllerWithRootViewController (UIApplication.SharedApplication.KeyWindow.RootViewController);
			if (topController != null) {
				// Present Alert
				topController.PresentViewController (okAlertController, true, null);
			}
		}


		/// <summary>
		/// Messages for select item.
		/// if(PopoverPresentationController)
		/// for select a view you need to add a tag "99"
		/// </summary>
		/// <param name="header">Header.</param>
		/// <param name="description">Description.</param>
		/// <param name="items">Items.</param>
		/// <param name="cancelClick">Cancel click.</param>
		/// <param name="parameters">Parameters.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void MessageForSelectItem<T> (string header, string description, System.Collections.Generic.List<MessageData<T>> items, Action cancelClick, params object[] parameters)
		{
			var okAlertController = UIAlertController.Create (header, description, UIAlertControllerStyle.ActionSheet);
			foreach (var item in items) {
				okAlertController.AddAction (UIAlertAction.Create (item.Title, UIAlertActionStyle.Default, alert => {
					if (item.Selected != null) {
						item.Selected (item.Data);
					}
				}));
			}
			okAlertController.AddAction (UIAlertAction.Create (CancelText, UIAlertActionStyle.Cancel, null));
			var topController = NavigationService.TopViewControllerWithRootViewController (UIApplication.SharedApplication.KeyWindow.RootViewController);
			// Required for iPad - You must specify a source for the Action Sheet since it is
			// displayed as a popover
			UIPopoverPresentationController presentationPopover = okAlertController.PopoverPresentationController;
			if (presentationPopover != null) {
				presentationPopover.SourceView = topController.View.ViewWithTag (99);
				presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Up;
			}
			topController.PresentViewController (okAlertController, true, null);
		}

		public void Dismiss ()
		{
			throw new NotImplementedException ();
		}

		public void Toast (string text, params object[] parameters)
		{
			throw new NotImplementedException ();
		}
			

		#endregion
	}
}

