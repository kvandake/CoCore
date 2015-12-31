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
				okAlertController.AddAction (UIAlertAction.Create (CancelText, UIAlertActionStyle.Default, alert => cancelClick ()));
			}
			var topController = NavigationService.TopViewControllerWithRootViewController (UIApplication.SharedApplication.KeyWindow.RootViewController);
			if (topController != null) {
				// Present Alert
				topController.PresentViewController (okAlertController, true, null);
			}
		}


		public void Toast (string text, params object[] parameters)
		{
			throw new NotImplementedException ();
		}
			

		#endregion
	}
}

