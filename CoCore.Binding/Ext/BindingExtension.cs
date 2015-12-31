using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Linq;

namespace CoCore.Binding
{
    internal static class BindingExtension
    {
        internal static object GetValue(object target, MemberInfo memberInfo)
        {
            var p = memberInfo as PropertyInfo;
            if (p != null)
            {
                return p.GetValue(target);
            }
            var f = memberInfo as FieldInfo;
            return f?.GetValue(target);
        }

        internal static bool SetValue(object target, MemberInfo memberInfo, object value)
        {
            var p = memberInfo as PropertyInfo;
            if (p != null)
            {
                p.SetValue(target, value);
                return true;
            }
            var f = memberInfo as FieldInfo;
            if (f == null) return false;
            f.SetValue(target, value);
            return true;
        }

        internal static EventInfo GetEvent(Type type, string eventName)
        {
            var t = type;
            while (t != null && t != typeof (object))
            {
                var ti = t.GetTypeInfo();
                var ev = t.GetTypeInfo().GetDeclaredEvent(eventName);
                if (ev != null)
                    return ev;
                t = ti.BaseType;
            }
            return null;
        }

        internal static Delegate CreateGenericEventHandler(EventInfo evt, Action d)
        {
            var handlerType = evt.EventHandlerType;
            var handlerTypeInfo = handlerType.GetTypeInfo();
            var handlerInvokeInfo = handlerTypeInfo.GetDeclaredMethod("Invoke");
            var eventParams = handlerInvokeInfo.GetParameters();

            var parameters = eventParams.Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToArray();
            var body = Expression.Call(Expression.Constant(d), d.GetType().GetTypeInfo().GetDeclaredMethod("Invoke"));
            var lambda = Expression.Lambda(body, parameters);

            var delegateInvokeInfo = lambda.Compile().GetMethodInfo();
            return delegateInvokeInfo.CreateDelegate(handlerType, null);
        }



        /// <summary>
        /// Gets the value of a Linq expression.
        /// </summary>
        /// <param name="expr">The expresssion.</param>
        internal static object EvalExpression(Expression expr)
        {
            //
            // Easy case
            //
            if (expr.NodeType == ExpressionType.Constant)
            {
                return ((ConstantExpression) expr).Value;
            }
            //
            // General case
            //
            var lambda = Expression.Lambda(expr, Enumerable.Empty<ParameterExpression>());
            return lambda.Compile().DynamicInvoke();
        }

        internal static object EvalTarget(MemberExpression expr)
        {
            // "descend" toward's the root object reference:
            var ex = expr.Expression;
            return EvalExpression(ex);
        }

        internal static MemberInfo GetMemberInfo(Expression expression)
        {
            if (expression == null)
            {
                return null;
            }
            var lambda = expression as LambdaExpression;
            if (lambda == null)
            {
                throw new ArgumentException("Invalid argument", nameof(expression));
            }
            var body = lambda.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("Invalid argument", nameof(expression));
            }
            return body.Member;
        }


    }
}

