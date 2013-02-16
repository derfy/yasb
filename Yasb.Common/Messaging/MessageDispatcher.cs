using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Concurrent;

namespace Yasb.Common.Messaging
{
    public delegate void MessageHandlerMethodDelegate(object target, object message);

    public class MsgHandlerInfo
    {
        public MsgHandlerInfo(Type msgType)
        {
            MessageHandlerGenericType = typeof(IHandleMessages<>).MakeGenericType(msgType);
            HandleMethod=CreateMessageHandlerDelegate(MessageHandlerGenericType.GetMethod("Handle"));
        }
        public Type MessageHandlerGenericType;
        public Type InitiatedByGenericType;
        public Type MessageType;
        public MessageHandlerMethodDelegate HandleMethod {get;private set;}
        /// <summary>
        /// Create a delegate for message handler (Handle) method of a generic interface
        /// Used by message bus for delivering messages to their handlers.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static MessageHandlerMethodDelegate CreateMessageHandlerDelegate(MethodInfo method)
        {
            ParameterExpression instanceParameter = Expression.Parameter(typeof(object), "target");
            ParameterExpression argumentsParameter = Expression.Parameter(typeof(object), "message");

            MethodCallExpression call = Expression.Call(
                Expression.Convert(instanceParameter, method.DeclaringType),
                method,
                Expression.Convert(argumentsParameter, method.GetParameters()[0].ParameterType)
                );
            Expression<MessageHandlerMethodDelegate> lambda = Expression.Lambda<MessageHandlerMethodDelegate>(
                call,
                instanceParameter,
                argumentsParameter);
            return lambda.Compile();
        }
    }
    public class MessageDispatcher
    {
        private ConcurrentDictionary<Type, MsgHandlerInfo> _handlers = new ConcurrentDictionary<Type, MsgHandlerInfo>();

        public MsgHandlerInfo GetHandlersFor(Type msgType)
        {
            MsgHandlerInfo mi;
            if (!_handlers.TryGetValue(msgType, out mi))
            {
                mi = new MsgHandlerInfo(msgType);
                _handlers[msgType] = mi;
            }
            return mi;
        }
    }
}
