using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Concurrent;

namespace Yasb.Common.Messaging
{
    public delegate IEnumerable<IHandleMessages> MessageHandlerFactory(Type type);
    public delegate void MessageHandlerMethodDelegate(object target, object message);
    public interface IMessageDispatcher 
    {
        void Dispatch(MessageEnvelope envelope);
    }
    public class MessageDispatcher<TConnection> : IMessageDispatcher
    {
        private MessageHandlerFactory _messageHandlerFactory;
        public MessageDispatcher(MessageHandlerFactory messageHandlerFactory)
        {
            _messageHandlerFactory = messageHandlerFactory;
        }
        public void Dispatch(MessageEnvelope envelope)
        {
            var messageType = envelope.Message.GetType();
            var handler = _messageHandlerFactory(messageType).Single(h => h.GetType().Equals(envelope.HandlerType));
            var mhi = GetHandleMethodDelegate(messageType);
            try { mhi(handler, envelope.Message); }
            catch (Exception ex)
            {
                throw new MessageHandlerException(envelope.Id, ex.Message);
            }
            
        }
        private ConcurrentDictionary<Type, MessageHandlerMethodDelegate> _handlers = new ConcurrentDictionary<Type, MessageHandlerMethodDelegate>();

        public MessageHandlerMethodDelegate GetHandleMethodDelegate(Type msgType)
        {
            MessageHandlerMethodDelegate mi;
            if (!_handlers.TryGetValue(msgType, out mi))
            {
                var messageHandlerGenericType = typeof(IHandleMessages<>).MakeGenericType(msgType);
                _handlers[msgType] = CreateMessageHandlerDelegate(messageHandlerGenericType.GetMethod("Handle"));
                return _handlers[msgType];
            }
            return mi;
        }

        private static MessageHandlerMethodDelegate CreateMessageHandlerDelegate(MethodInfo method)
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
}
