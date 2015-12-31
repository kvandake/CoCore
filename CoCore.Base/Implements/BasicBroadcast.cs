using System;
using System.Collections.Generic;
using System.Reflection;

namespace CoCore.Base
{
	public class BasicBroadcast : IBroadcast
	{
	    readonly ViewModelBase _vm;
		Dictionary<Type,List<BroadcastItem>> _actions;
		static readonly object RegisterLock = new object ();
		static readonly object UnRegisterLock = new object ();
		static readonly object SendLock = new object ();

		public Dictionary<Type, List<BroadcastItem>> Actions => _actions ?? (_actions = new Dictionary<Type, List<BroadcastItem>> ());

	    static BroadcastContainer Container {
			get {
				var container = BootstrapBase.Locator.IocContainer.Resolve<BroadcastContainer> ();
			    if (container != null) return container;
			    container = new BroadcastContainer ();
			    BootstrapBase.Locator.IocContainer.Register (() => container);
			    return container;
			}
		}


		public BasicBroadcast(ViewModelBase vm){
			_vm = vm;
		}

		#region IBroadcast implementation

		public void SubscribeOnBroadcast ()
		{
			Container [_vm.GetType ()] = this;
		}

		public void UnscribeOnBroadcast ()
		{
			var type = _vm.GetType ();
			if (Container.ContainsKey (type)) {
				Container.Remove (type);
			}
		}

		public void Register<T> (Action<T> action, string token = null)
		{
			lock (RegisterLock) {
				var item = new BroadcastItemGeneric<T> (action, token);
				var type = typeof(T);
				List<BroadcastItem> listActions;
				if (Actions.ContainsKey (type)) {
					listActions = Actions [type];
				} else {
					listActions = new List<BroadcastItem> ();
					Actions [type] = listActions;
				}
				listActions.Add (item);
			}
		}

		public void UnRegister<T> (string token = null)
		{
			lock (UnRegisterLock) {
				var type = typeof(T);
				if (Actions.ContainsKey (type)) {
					var listActions = Actions [type];
					if (listActions != null) {
						var findActions = listActions.FindAll (x => x.Key == token);
					    foreach (var item in findActions) {
					        listActions.Remove (item);
					    }
					}
				}
			}
		}

		public void Send<T> (T obj, string token = null)
		{
			lock (SendLock) {
				var type = typeof(T);
				foreach (var item in Container) {
					SendToOtherBroadcast (item.Value, type, obj, token);
				}
			}
		}

		static void SendToOtherBroadcast<T>(BasicBroadcast broadcast,Type type, T obj,string token){
			if (broadcast.Actions.ContainsKey (type)) {
				var listActions = broadcast.Actions [type];
				if (listActions != null) {
					var findActions = listActions.FindAll (x => x.Key == token);
				    foreach (var item in findActions) {
				        item.Execute (obj);
				    }
				}
			}
		}
			
		#endregion


		/// <summary>
		/// Broadcast item.
		/// </summary>
		public class BroadcastItem{
			
			public string Key { get; }

			Action _staticAction;

			/// <summary>
			/// Gets or sets the <see cref="MethodInfo" /> corresponding to this WeakAction's
			/// method passed in the constructor.
			/// </summary>
			protected MethodInfo Method
			{
				get;
				set;
			}

			/// <summary>
			/// Gets the name of the method that this WeakAction represents.
			/// </summary>
			public virtual string MethodName => _staticAction != null ? _staticAction.GetMethodInfo ().Name : Method.Name;


		    /// <summary>
			/// Gets or sets a WeakReference to the target passed when constructing
			/// the WeakAction. This is not necessarily the same as
			/// method is anonymous.
			/// </summary>
			protected WeakReference Reference
			{
				get;
				set;
			}

			/// <summary>
			/// Gets a value indicating whether the WeakAction is static or not.
			/// </summary>
			public virtual bool IsStatic => _staticAction != null;

		    /// <summary>
			/// Initializes an empty instance of the <see cref="BroadcastItem" /> class.
			/// </summary>
			protected BroadcastItem()
			{
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="BroadcastItem" /> class.
			/// </summary>
			/// <param name="action">Action.</param>
			/// <param name="key">Key.</param>
			public BroadcastItem(Action action,string key)
				: this(action?.Target, action,key)
			{
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="BroadcastItem" /> class.
			/// </summary>
			/// <param name="target">Target.</param>
			/// <param name="action">Action.</param>
			/// <param name="key">Key.</param>
			public BroadcastItem(object target, Action action,string key):this(key)
			{
				var methodInfo = action.GetMethodInfo();
				if (methodInfo.IsStatic)
				{
					_staticAction = action;
					if (target != null)
					{
						// Keep a reference to the target to control the
						// WeakAction's lifetime.
						Reference = new WeakReference(target);
					}
					return;
				}
				Method = methodInfo;
				Reference = new WeakReference(target);
			}

			public BroadcastItem(string key){
				Key = key;
			}

			/// <summary>
			/// Gets a value indicating whether the Action's owner is still alive, or if it was collected
			/// by the Garbage Collector already.
			/// </summary>
			public virtual bool IsAlive
			{
				get
				{
					if (_staticAction == null
						&& Reference == null)
					{
						return false;
					}

					if (IsStatic)
					{
						return Reference == null || Reference.IsAlive;

					}

					return Reference.IsAlive;
				}
			}

			/// <summary>
			/// Gets the Action's owner. This object is stored as a 
			/// <see cref="WeakReference" />.
			/// </summary>
			public object Target => Reference?.Target;

		    /// <summary>
			/// Executes the action. This only happens if the action's owner
			/// is still alive.
			/// </summary>
			public virtual void Execute()
			{
				if (IsStatic)
				{
					_staticAction();
					return;
				}
				var actionTarget = Target;
				if (IsAlive)
				{
					if (Method != null
						&& actionTarget != null)
					{
						Method.Invoke(actionTarget, null);
					}
				}
			}

			/// <summary>
			/// Executes the action. This only happens if the action's owner
			/// is still alive.
			/// </summary>
			/// <param name="parameter">A parameter to be passed to the action.</param>
			public virtual void Execute(object parameter)
			{
				var actionTarget = Target;
				if (IsAlive) {
					if (Method != null
						&& Reference != null
						&& actionTarget != null) {
						Method.Invoke (
							actionTarget,
							new [] {
								parameter
							});
					}
				}
			}

			/// <summary>
			/// Sets the reference that this instance stores to null.
			/// </summary>
			public void MarkForDeletion()
			{
				Reference = null;
				Method = null;
				_staticAction = null;
			}
		}

		public class BroadcastItemGeneric<T> : BroadcastItem{

			Action<T> _staticAction;

			/// <summary>
			/// Gets the name of the method that this WeakAction represents.
			/// </summary>
			public override string MethodName => _staticAction != null ? _staticAction.GetMethodInfo ().Name : Method.Name;

		    public override void Execute ()
			{
				if (IsStatic) {
					_staticAction (default(T));
					return;
				}
				base.Execute ();
			}

			public override void Execute (object parameter)
			{
				if (IsStatic) {
					_staticAction ((T)parameter);
					return;
				}
				base.Execute (parameter);
			}

			/// <summary>
			/// Gets a value indicating whether the Action's owner is still alive, or if it was collected
			/// by the Garbage Collector already.
			/// </summary>
			public override bool IsAlive {
				get {
					if (_staticAction == null
						&& Reference == null) {
						return false;
					}

					if (IsStatic) {
						return Reference == null || Reference.IsAlive;
					}
					return Reference.IsAlive;
				}
			}

			public override bool IsStatic => _staticAction != null;

		    /// <summary>
			/// Initializes a new instance of the WeakAction class.
			/// </summary>
			/// <param name="action">Action.</param>
			/// <param name="key">Key.</param>
			public BroadcastItemGeneric(Action<T> action,string key)
				: this(action?.Target, action,key)
			{
			}

			/// <summary>
			/// Initializes a new instance of the WeakAction class.
			/// </summary>
			/// <param name="target">Target.</param>
			/// <param name="action">Action.</param>
			/// <param name="key">Key.</param>
			public BroadcastItemGeneric(object target, Action<T> action,string key):base(key)
			{
				var methodInfo = action.GetMethodInfo();
				if (methodInfo.IsStatic)
				{
					_staticAction = action;

					if (target != null)
					{
						// Keep a reference to the target to control the
						// WeakAction's lifetime.
						Reference = new WeakReference(target);
					}

					return;
				}
				Method = methodInfo;
				Reference = new WeakReference(target);
			}



			/// <summary>
			/// Sets all the actions that this WeakAction contains to null,
			/// which is a signal for containing objects that this WeakAction
			/// should be deleted.
			/// </summary>
			public new void MarkForDeletion()
			{
				_staticAction = null;
				base.MarkForDeletion();
			}
		}

		/// <summary>
		/// Broadcast container.
		/// </summary>
		class BroadcastContainer : Dictionary<Type,BasicBroadcast>{
		}
			
	}
}

