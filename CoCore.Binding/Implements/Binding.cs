using System;
using System.Reflection;
using System.ComponentModel;

namespace CoCore.Binding
{
    public class Binding
    {
        /// <summary>
        /// The source at the "top" of the property chain.
        /// </summary>
        internal WeakReference<Trigger> TriggerSource;

        /// <summary>
        /// The target at the "top" of the property chain.
        /// </summary>
        internal WeakReference<Trigger> TriggerTarget;

        /// <summary>
        /// Gets the source object for the binding.
        /// </summary>
        internal Trigger Source
        {
            get
            {
                Trigger trigger;
                return TriggerSource == null ? null : TriggerSource.TryGetTarget(out trigger) ? trigger : null;
            }
        }

        /// <summary>
        /// Gets the target object for the binding.
        /// </summary>
        internal Trigger Target
        {
            get
            {
                Trigger trigger;
                return TriggerTarget == null ? null : TriggerTarget.TryGetTarget(out trigger) ? trigger : null;
            }
        }


        internal Binding(Trigger triggerSource, Trigger triggerTarget)
        {
            TriggerSource = new WeakReference<Trigger>(triggerSource);
            TriggerTarget = new WeakReference<Trigger>(triggerTarget);
        }


        internal void SubscribeSourceFromChangeNotificationEvent(string eventName, bool isNotifyPropertyChange)
        {
            var source = Source;
            if (source != null)
            {
                if (isNotifyPropertyChange)
                {
                    var npc = source.Target as INotifyPropertyChanged;
                    if (npc != null && (source.Member is PropertyInfo))
                    {
                        npc.PropertyChanged += HandleSourcePropertySourceChanged;
                    }
                }
                if (!string.IsNullOrEmpty(eventName))
                {
                    source.Subscribe(eventName, HandleSourceEvent);
                }
            }
        }


        internal void SubscribeTargetFromChangeNotificationEvent(string eventName, bool isNotifyPropertyChange)
        {
            var target = Target;
            if (target != null)
            {
                if (isNotifyPropertyChange)
                {
                    var npc = target.Target as INotifyPropertyChanged;
                    if (npc != null && (target.Member is PropertyInfo))
                    {
                        npc.PropertyChanged += HandleTargetPropertySourceChanged;
                    }
                }
                if (!string.IsNullOrEmpty(eventName))
                {
                    target.Subscribe(eventName, HandleTargetEvent);
                }
            }
        }

        internal void UnsubscribeSourceSourceFromChangeNotificationEvent()
        {
            var source = Source;
            if (source != null)
            {
                source.UnSubscribe();
                var npc = source.Target as INotifyPropertyChanged;
                if (npc != null && (source.Member is PropertyInfo))
                {
                    npc.PropertyChanged -= HandleSourcePropertySourceChanged;
                    return;
                }
                TriggerSource = null;
            }
        }

        internal void UnsubscribeTargetSourceFromChangeNotificationEvent()
        {
            var target = Target;
            if (target != null)
            {
                target.UnSubscribe();
                var npc = target.Target as INotifyPropertyChanged;
                if (npc != null && (target.Member is PropertyInfo))
                {
                    npc.PropertyChanged -= HandleSourcePropertySourceChanged;
                    return;
                }
                TriggerTarget = null;
            }
        }


        internal void InvalidateMember(Trigger getter, Trigger setter)
        {
            getter.SetValue(setter.GetValue());
        }




        public virtual void UnSubscribe()
        {
            UnsubscribeSourceSourceFromChangeNotificationEvent();
            UnsubscribeTargetSourceFromChangeNotificationEvent();
        }



        #region UpdateMethods


        protected void HandleSourcePropertySourceChanged(object sender, PropertyChangedEventArgs e)
        {
            var source = Source;
            var target = Target;
            if (source != null && target != null)
            {
                InvalidateMember(target, source);
            }
        }

        protected void HandleTargetPropertySourceChanged(object sender, PropertyChangedEventArgs e)
        {
            var source = Source;
            var target = Target;
            if (source != null && target != null)
            {
                InvalidateMember(source, target);
            }
        }

        protected void HandleSourceEvent(object sender, EventArgs e)
        {
            var target = Target;
            var source = Source;
            if (target != null && source != null)
            {
                var valueLocal = source.GetValue();
                var targetValue = target.GetValue();
                if (Equals(valueLocal, targetValue))
                {
                    return;
                }
                target.SetValue(valueLocal);
            }
        }

        protected void HandleTargetEvent(object sender, EventArgs e)
        {
            var target = Target;
            var source = Source;
            if (target != null && source != null)
            {
                var valueLocal = target.GetValue();
                var sourceValue = source.GetValue();
                if (Equals(valueLocal, sourceValue))
                {
                    return;
                }
                source.SetValue(valueLocal);
            }
        }


        #endregion

    }
}

