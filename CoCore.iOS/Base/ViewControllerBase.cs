using CoCore.Base;
using UIKit;

namespace CoCore.iOS
{
	public class ViewControllerBase<T> : UIViewController,IReturnWithResult where T: ViewModelBase,new()
	{


		public Intent Context { get; set;}


		T _dataContext;


		public T DataContext {
			get {
				return _dataContext ?? (_dataContext = new T ());
			}
		}

		public override void ViewDidAppear (bool animated)
		{
			DataContext.ViewDidAppear ();
			base.ViewDidAppear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			DataContext.ViewDidDisappear ();
			base.ViewDidDisappear (animated);
		}


		#region IReturnWithResult implementation

		public virtual bool ReturnWithResult (Intent intent)
		{
			return true;
		}

		#endregion
	}
}

