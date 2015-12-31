using System;
using UIKit;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using CoCore.Base;

namespace CoCore.iOS
{
	public class NavigationService : INavigation
	{

		public const string ToPresent =@"presenting";
		public const string ToPush =@"pushing";




		bool _useAnimated =true;
	    readonly UIWindow _window;
		IDictionary<string,Type> _maps;
		IDictionary<string,Tuple<MethodInfo,object>> _converts;

		string _toPresent = ToPresent;
		string _toPush = ToPush;

		public IDictionary<string, Tuple<MethodInfo, object>> Converts {
			get {
				return _converts ?? (_converts = new Dictionary<string, Tuple<MethodInfo, object>> ());
			}
		}

		public IDictionary<string, Type> Maps {
			get {
				return _maps ?? (_maps = new Dictionary<string,Type> ());
			}
		}



		public bool UseAnimated {
			get {
				return _useAnimated;
			}
			set {
				_useAnimated = value;
			}
		}

		public NavigationService(UIWindow window){
			_window = window;
		}


		public void Start(UIViewController view){
			_window.RootViewController = view;
			_window.MakeKeyAndVisible ();

		}

		public void Register (string name, Type viewType)
		{
			Maps [name] = viewType;
		}

		public void Remove (string name)
		{
			if (Maps.ContainsKey (name)) {
				Maps.Remove (name);
			}
		}


		public void ToPushParameter(string customParameter){
			_toPush = customParameter;
		}

		public void ToPresentParameter(string customParameter){
			_toPresent = customParameter;
		}



		static T CreateByConvert<T>(Tuple<MethodInfo,object> convert,object view){
			var obj = convert.Item1.Invoke (convert.Item2 == null || convert.Item1.IsStatic ? new object () : convert.Item2, new[]{ view });
			return (T)obj;
		}

		public void ConvertView<T> (Func<T,T> convert, string customParameter)
		{
			var methodInfo = convert.GetMethodInfo ();
			var target = convert.Target;
			Converts [customParameter] = new Tuple<MethodInfo, object> (methodInfo, target);
		}

		public void Navigate (string name, Intent intent, params string[] parameters)
		{
			var topVc = TopViewController;
			var navVc = topVc.NavigationController;
			if (!Maps.ContainsKey (name)) {
				throw new KeyNotFoundException (name);
			}
			var type = Maps [name];
			bool isPush = false;
			bool isPresent = false;
			Tuple<MethodInfo,object> convert = null;
			foreach (var item in parameters) {
				isPush |= _toPush == item;
				isPresent |= _toPresent == item;
				convert = Converts.FirstOrDefault (x => x.Key == item).Value;
			}
			var view = Create (type, intent) as UIViewController;
			if (convert != null) {
				view = CreateByConvert<UIViewController> (convert, view);
			}
			if (view == null) {
				throw new TypeInitializationException (type.FullName, new Exception ("does not initialize"));
			}
			if (intent != null)
			{
			    if (isPush && navVc != null) {
					navVc.PushViewController (view, UseAnimated);
					return;
				}
			    if (isPresent) {
			        topVc.PresentViewController (view, UseAnimated, null);
			        return;
			    }
			}
		    if (navVc != null) {
				if (navVc.GetType () == view.GetType ()) {
					throw new ArgumentException ("The navigation controller does not push to the navigation controller");
				}
				navVc.PushViewController (view, UseAnimated);
			} else {
				topVc.PresentViewController (view, UseAnimated, null);
			}
		}

		public bool GoBackWithResult (Intent intent)
		{
			var topVc = TopViewController;
			if (topVc == null)
				return false;
			var navVc = topVc.NavigationController;
			if (navVc != null) {
				bool result;
				if (navVc.PresentingViewController != null) {
					result = SendToResult (navVc.PresentingViewController, intent);
					navVc.DismissViewController (UseAnimated, null);
					return result;
				}
				var vControllers = navVc.ViewControllers;
				result = false;
				if (vControllers.Length > 1) {
					var vcBack = vControllers [vControllers.Length - 2];
					result = SendToResult (vcBack, intent);
				}
				navVc.PopViewController (UseAnimated);
				return result;
			}
			var vc = BackViewControllerWithRootViewController (topVc.PresentingViewController);
			topVc.DismissViewController (UseAnimated, null);
			return SendToResult (vc, intent);
		}



		public bool GoBack ()
		{
			var topVc = TopViewController;
			if (topVc == null)
				return false;
			var navVc = topVc.NavigationController;
			if (navVc != null) {
				if (navVc.PresentingViewController != null) {
					navVc.DismissViewController (UseAnimated, null);
					return true;
				}
				return navVc.PopViewController (UseAnimated) != null;
			}
			topVc.DismissViewController (UseAnimated, null);
			return true;
		}
			


		#region Get View Controller

		internal UIViewController TopViewController {
			get {
				return TopViewControllerWithRootViewController (_window.RootViewController);
			}
		}


		internal static UIViewController TopViewControllerWithRootViewController(UIViewController rootViewController){
			if (rootViewController == null) {
				throw new NullReferenceException ("rootViewController is null");
			}
			var uITabBarController = rootViewController as UITabBarController;
			if (uITabBarController != null) {
				return TopViewControllerWithRootViewController (uITabBarController.SelectedViewController);
			} else {
				var uINavigationController = rootViewController as UINavigationController;
				if (uINavigationController != null) {
					return TopViewControllerWithRootViewController (uINavigationController.VisibleViewController);
				} else if (rootViewController.PresentedViewController != null) {
					UIViewController presentedViewController = rootViewController.PresentedViewController;
					return TopViewControllerWithRootViewController (presentedViewController);
				} else {
					return rootViewController;
				}
			}
		}

		internal static UIViewController BackViewControllerWithRootViewController(UIViewController rootViewController){
			if (rootViewController == null) {
				throw new NullReferenceException ("rootViewController is null");
			}
			var uITabBarController = rootViewController as UITabBarController;
			if (uITabBarController != null) {
				return BackViewControllerWithRootViewController (uITabBarController.SelectedViewController);
			} else {
				var uINavigationController = rootViewController as UINavigationController;
				return uINavigationController != null 
					? BackViewControllerWithRootViewController (uINavigationController.TopViewController) 
						: rootViewController;
			}
		}
			

		#endregion

		static bool SendToResult(UIViewController vc, Intent intent){
			var resultInt = vc as IReturnWithResult;
			return resultInt != null && resultInt.ReturnWithResult (intent);
		}


		static object Create (Type type, Intent intent)
		{
			var view = Extension.CreateForType (type, intent == null ? null : intent.Values);
			//var mvvmView = view as IContext;
			//if (mvvmView != null) {
			//	if (!intent.RemoveToNavigate) {
			//		mvvmView.Context = intent;
			//	}
			//	return mvvmView;
			//}
			return view;
		}

	}
}

