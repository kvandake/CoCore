using System;
using CoCore.Base;
using Foundation;
using UIKit;

namespace CoCore.iOS
{
	public abstract class ViewControllerBase<T> : UIViewController,IReturnWithResult,IContext where T: ViewModelBase,new()
	{
        protected ViewControllerBase()
	    {
	    }

        protected ViewControllerBase(NSCoder coder) : base(coder)
	    {
	    }

        protected ViewControllerBase(NSObjectFlag t) : base(t)
	    {
	    }

        protected ViewControllerBase(IntPtr handle) : base(handle)
	    {
	    }

	    protected ViewControllerBase(string nibName, NSBundle bundle) : base(nibName, bundle)
	    {
	    }




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

