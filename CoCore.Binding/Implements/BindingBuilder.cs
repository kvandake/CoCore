using System;
using System.Linq.Expressions;

namespace CoCore.Binding
{
    public class BindingBuilder
    {
        private bool _isSourcePropertyChangeEnable = true;
        private bool _isTargetPropertyChangeEnable = true;
        private BindingMode _bindingMode = BindingMode.OneWay;
        private string _customSourceEventName;
        private string _customTargetEventName;
        private Expression _sourcePropertyExpression;
        private Expression _targetPropertyExpression;
        private object _sourceRoot;
        private object _targetRoot;


        public static BindingBuilder Create<TSource, TTarget>(object source, Expression<Func<TSource>> sourceExpression,
            object target, Expression<Func<TTarget>> targetExpression)
        {
            var builder = new BindingBuilder();
            builder
                .SetSource(source, sourceExpression)
                .SetTarget(target, targetExpression);
            return builder;
        }

        public BindingBuilder SetSource<TSource>(object source, Expression<Func<TSource>> sourceExpression)
        {
            _sourceRoot = source;
            _sourcePropertyExpression = sourceExpression;
            return this;
        }

        public BindingBuilder SetTarget<TTarget>(object target, Expression<Func<TTarget>> targetExpression)
        {
            _targetRoot = target;
            _targetPropertyExpression = targetExpression;
            return this;
        }


        public BindingBuilder SetTarget<TTarget>(Expression<Func<TTarget>> targetExpression)
        {
            var member = targetExpression.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException("targetExpression is not memberExpression");
            }
            _targetPropertyExpression = targetExpression;
            return SetTarget(BindingExtension.EvalTarget(member), targetExpression);
        }

        public BindingBuilder SetBinding(BindingMode mode)
        {
            _bindingMode = mode;
            return this;
        }

        public BindingBuilder WithSourcePropertyChange(bool enable)
        {
            _isSourcePropertyChangeEnable = enable;
            return this;
        }

        public BindingBuilder WithTargetPropertyChange(bool enable)
        {
            _isTargetPropertyChangeEnable = enable;
            return this;
        }

        public BindingBuilder UpdateSourceTrigger(string eventName)
        {
            _customSourceEventName = eventName;
            return this;
        }

        public BindingBuilder UpdateTargetTrigger(string eventName)
        {
            _customTargetEventName = eventName;
            return this;
        }


        public Binding Build()
        {
            if (_targetRoot == null || _sourceRoot == null)
            {
                throw new ArgumentException("targetRoot or sourceRoot are null");
            }
            if (_targetPropertyExpression == null || _sourcePropertyExpression == null)
            {
                throw new ArgumentException("targetPropertyExpression or sourcePropertyExpression are null");
            }
            var sourceMember = BindingExtension.GetMemberInfo(_sourcePropertyExpression);
            var sourceTrigger = new Trigger(_sourceRoot, sourceMember);

            var targetMember = BindingExtension.GetMemberInfo(_targetPropertyExpression);
            var targetTrigger = new Trigger(_targetRoot, targetMember);

            var binding = new Binding(sourceTrigger, targetTrigger);
            binding.SubscribeSourceFromChangeNotificationEvent(_customSourceEventName, _isSourcePropertyChangeEnable);
            if (_bindingMode == BindingMode.TwoWay)
            {
                binding.SubscribeTargetFromChangeNotificationEvent(_customTargetEventName, _isTargetPropertyChangeEnable);
            }
            return binding;
        }

    }
}

