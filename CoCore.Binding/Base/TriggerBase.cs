using System;
using System.Collections.Generic;
using System.Reflection;

namespace CoCore.Binding
{
    public abstract class TriggerBase
    {
        public object Target;
        private IDictionary<string, Tuple<EventInfo, Delegate>> _events;

        protected IDictionary<string, Tuple<EventInfo, Delegate>> Events
            => _events ?? (_events = new Dictionary<string, Tuple<EventInfo, Delegate>>());

        protected TriggerBase(object target)
        {
            Target = target;
        }

        #region ISubscribe implementation

        public void Subscribe(string eventName, EventHandler handler)
        {
            var type = Target.GetType();
            EventInfo ev = BindingExtension.GetEvent(type, eventName);
            if (ev == null) return;
            var isClassicHandler =
                typeof (EventHandler).GetTypeInfo().IsAssignableFrom(ev.EventHandlerType.GetTypeInfo());
            var eventHandler = isClassicHandler
                ? handler
                : BindingExtension.CreateGenericEventHandler(ev, () => handler(null, EventArgs.Empty));
            ev.AddEventHandler(Target, eventHandler);
            Events.Add(eventName, new Tuple<EventInfo, Delegate>(ev, eventHandler));
        }

        public void UnSubscribe()
        {
            foreach (var item in Events)
            {
                item.Value.Item1.RemoveEventHandler(Target, item.Value.Item2);
            }
        }

        #endregion


    }
}

