using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CoCore.Base
{
	public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
	{

		string title;
		public virtual string Title {
			get {
				return title;
			}
			set {
				title = value;
				OnPropertyChanged ();
			}
		}

	    IBroadcast _broadcast;

		#region INotifyPropertyChanged implementation

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion


		public virtual void ViewDidAppear(){
		}

		public virtual void ViewDidDisappear(){

		}
			
		/// <summary>
		/// Gets the broadcast.
		/// </summary>
		public virtual IBroadcast Broadcast => _broadcast ?? (_broadcast = new BasicBroadcast (this));

	    /// <summary>
		/// Find Default Messages the center.
		/// </summary>
		public IMessagePresenter MessageCenter => BootstrapBase.Locator.IocContainer.Resolve<IMessagePresenter> ();


	    public T Resolve<T>(string name = null){
			return BootstrapBase.Locator.IocContainer.Resolve<T> (name);
		}


		/// <summary>
		/// Find Messages the center.
		/// </summary>
		/// <returns>The center.</returns>
		/// <param name="name">Name.</param>
		public IMessagePresenter SpecificMessageCenter(string name = null){
			return BootstrapBase.Locator.IocContainer.Resolve<IMessagePresenter> (name);
		}



		/// <summary>
		/// Create the intent.
		/// </summary>
		/// <returns>The intent.</returns>
		public Intent CreateIntent(){
			var nav = BootstrapBase.Locator.IocContainer.Resolve<INavigation> ();
			return new Intent (nav);
		}


		protected virtual void OnPropertyChanged ([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
		    handler?.Invoke (this, new PropertyChangedEventArgs (propertyName));
		}


		public void Dispose ()
		{
			_broadcast = null;
		}
	}
}

