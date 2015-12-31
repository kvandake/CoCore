using System;
using System.Windows.Input;

namespace CoCore.Binding
{
    public class ControlCommand : TriggerBase
    {
        public ControlCommand(object target, string eventName, ICommand command) : base(target)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                throw new ArgumentException("string is null or empty", nameof(eventName));
            }
            Subscribe(eventName, (s, e) =>
            {
                if (command.CanExecute(null))
                {
                    command.Execute(null);
                }
            });
        }
    }
}

