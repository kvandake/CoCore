using System;
using System.Windows.Input;

namespace CoCore.Base
{
	public class WeakRelayCommand : ICommand
	{
	    readonly WeakReference<Func<object, bool>> _canExecute;
		readonly WeakReference<Action<object>> _executeAction;

		public WeakRelayCommand(Action<object> executeAction)
			: this(executeAction, null)
		{
		}

		public WeakRelayCommand(Action<object> executeAction, Func<object, bool> canExecute)
		{
			if (executeAction == null)
			{
				throw new ArgumentNullException(nameof(executeAction));
			}
			_executeAction =new WeakReference<Action<object>>(executeAction);
			_canExecute =new WeakReference<Func<object, bool>>(canExecute);
		}

		public bool CanExecute(object parameter)
		{
			bool result = true;

			Func<object, bool> canExecuteHandler;
			if (_canExecute.TryGetTarget (out canExecuteHandler)) {
				if (canExecuteHandler != null) {
					result = canExecuteHandler (parameter);
				}
				return result;
			}
			return true;
		}

		public event EventHandler CanExecuteChanged;

		public void RaiseCanExecuteChanged()
		{
			EventHandler handler = CanExecuteChanged;
		    handler?.Invoke(this, new EventArgs());
		}

		public void Execute(object parameter)
		{
			Action<object> action;
			if (_executeAction.TryGetTarget (out action)) {
				action (parameter);
			}
		}
	}
}

