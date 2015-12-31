using System;

namespace CoCore.Base
{
	public abstract class BootstrapBuilderBase : IDisposable
	{

		protected bool IsMessagePresenterBase = true;
		protected WeakReference<IIocContainer> WeakIocContainer;
		protected WeakReference<INavigation> WeakNavigation;
		protected WeakReference<IMessagePresenter> WeakMessagePresenter;

		protected BootstrapBuilderBase (IIocContainer container)
		{
			WeakIocContainer = new WeakReference<IIocContainer>(container);
		}


		/// <summary>
		/// Withs the base message center (default is true)
		/// </summary>
		/// <returns>The message center.</returns>
		/// <param name="enable">If set to <c>true</c> enable.</param>
		public BootstrapBuilderBase WithMessageCenterDefault(bool enable){
			IsMessagePresenterBase = enable;
			return this;
		}

		/// <summary>
		/// Withs the message center.
		/// </summary>
		/// <returns>The message center.</returns>
		/// <param name="messagePresenter">Message presenter.</param>
		public BootstrapBuilderBase WithMessageCenter(IMessagePresenter messagePresenter){
			WeakMessagePresenter = new WeakReference<IMessagePresenter>(messagePresenter);
			return this;
		}


		public BootstrapBuilderBase WithNavigation(INavigation navigation){
			WeakNavigation = new WeakReference<INavigation>(navigation);
			return this;
		}



		public abstract BootstrapBase Build();




		#region IDisposable implementation
		public void Dispose ()
		{
			WeakIocContainer = null;
			WeakNavigation = null;
			WeakMessagePresenter = null;
		}
		#endregion
	}
}

