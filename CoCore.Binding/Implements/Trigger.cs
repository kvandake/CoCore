using System.Reflection;

namespace CoCore.Binding
{
    internal class Trigger : TriggerBase
    {
        public MemberInfo Member;

        internal Trigger(object target, MemberInfo member) : base(target)
        {
            Member = member;
        }

        public object GetValue()
        {
            return BindingExtension.GetValue(Target, Member);
        }

        public bool SetValue(object value)
        {
            return BindingExtension.SetValue(Target, Member, value);
        }
    }
}

