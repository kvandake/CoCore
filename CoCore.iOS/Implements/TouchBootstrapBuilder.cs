using System;
using CoCore.Base;

namespace CoCore.iOS
{
	public class TouchBootstrapBuilder : BootstrapBuilderBase
	{

		public TouchBootstrapBuilder(IIocContainer container):base(container){
		}
			
		#region implemented abstract members of BootstrapBuilderBase

		public override BootstrapBase Build ()
		{
			IIocContainer container;
			INavigation navigation;
			IMessagePresenter messagePresenter;
			if (WeakIocContainer == null || !WeakIocContainer.TryGetTarget (out container)) {
				throw new NullReferenceException ("the container is null");
			}
			if (WeakNavigation != null && WeakNavigation.TryGetTarget (out navigation)) {
				container.Register (() => navigation);
			}
			if (WeakMessagePresenter != null && WeakMessagePresenter.TryGetTarget (out messagePresenter)) {
				container.Register (() => messagePresenter);
			}
			if (IsMessagePresenterBase && WeakMessagePresenter == null) {
				container.Register (() => new MessagePresenter ());
			}
			var touchBuilder = new TouchBootstrap (container);
			return touchBuilder;
		}

		#endregion




	}
}

