using System;
using System.Windows.Input;
using System.Threading.Tasks;

namespace CoCore.Base
{
	public class RelayCommand : ICommand
	{

		public event EventHandler CanExecuteChanged;

		#region Private Data Members

		/// <summary>
		/// Flag indicates that the command should be run on a seperate thread
		/// </summary>
		readonly bool _mRunOnBackGroudThread;
		/// <summary>
		/// Predicate that that evaluates if this command can be executed
		/// </summary>
		readonly Predicate<object> _mCanExecutePredicate;
		/// <summary>
		/// Action to be taken when this command is executed
		/// </summary>
		readonly Action<object> _mExecuteAction;
		/// <summary>
		/// Run when action method is complete and run on a seperate thread
		/// </summary>
		readonly Action<object> _mExecuteActionComplete;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="RelayCommand"/> class.
		/// </summary>
		/// <param name="canExecutePredicate">The can execute predicate.</param>
		/// <param name="executeAction">The execute action.</param>
		/// <param name="executeActionComplete">The execute action complete.</param>
		public RelayCommand(Predicate<object> canExecutePredicate, Action<object> executeAction, Action<object> executeActionComplete)
			: this(canExecutePredicate, executeAction, true)
		{
			if (executeActionComplete == null)
			{
				throw new ArgumentNullException(nameof(executeActionComplete));
			}
			_mExecuteActionComplete = executeActionComplete;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RelayCommand"/> class.
		/// </summary>
		/// <param name="canExecutePredicate">The can execute predicate.</param>
		/// <param name="executeAction">The execute action.</param>
		/// <param name="runOnBackGroundTread">if set to <c>true</c> [run on back ground tread].</param>
		public RelayCommand(Predicate<object> canExecutePredicate, Action<object> executeAction, bool runOnBackGroundTread)
			: this(canExecutePredicate, executeAction)
		{
			_mRunOnBackGroudThread = runOnBackGroundTread;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RelayCommand"/> class.
		/// </summary>
		/// <param name="canExecutePredicate">The can execute predicate.</param>
		/// <param name="executeAction">The execute action.</param>
		public RelayCommand(Predicate<object> canExecutePredicate, Action<object> executeAction)
		{
			if (canExecutePredicate == null)
			{
				throw new ArgumentNullException(nameof(canExecutePredicate));
			}

			if (executeAction == null)
			{
				throw new ArgumentNullException(nameof(executeAction));
			}

			_mCanExecutePredicate = canExecutePredicate;
			_mExecuteAction = executeAction;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RelayCommand"/> class.
		/// </summary>
		/// <param name="executeAction">The execute action.</param>
		public RelayCommand(Action<object> executeAction)
			: this(n => true, executeAction)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RelayCommand"/> class.
		/// </summary>
		/// <param name="executeAction">Execute action.</param>
		/// <param name="runOnBackGroundTread">If set to <c>true</c> run on back ground tread.</param>
		public RelayCommand(Action<object> executeAction,bool runOnBackGroundTread)
			: this(n => true, executeAction,runOnBackGroundTread)
		{

		}

		#endregion

		#region ICommand Members


		/// <summary>
		/// Defines the method that determines whether the command can execute in its current state.
		/// </summary>
		/// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		/// <returns>
		/// true if this command can be executed; otherwise, false.
		/// </returns>
		public bool CanExecute(object parameter)
		{
			return _mCanExecutePredicate(parameter);
		}

		/// <summary>
		/// Defines the method to be called when the command is invoked.
		/// </summary>
		/// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		public void Execute(object parameter)
		{
				if (!_mRunOnBackGroudThread)
				{
					_mExecuteAction(parameter);
				}
				else
				{
					if (_mExecuteActionComplete != null)
					{
						//Run with continuation
						var context = TaskScheduler.FromCurrentSynchronizationContext();
						Task.Factory.StartNew(_mExecuteAction, parameter).ContinueWith(_mExecuteActionComplete, context);
					}
					else
					{
						//Run as fire and forget
						Task.Factory.StartNew(_mExecuteAction, parameter);
					}
				}
		}

		#endregion


		protected virtual void OnCanExecuteChanged (EventArgs e)
		{
			var handler = CanExecuteChanged;
		    handler?.Invoke (this, e);
		}
	}
}

