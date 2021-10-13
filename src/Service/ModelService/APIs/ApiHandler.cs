using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Raid.Service.Messages;
using SuperSocket.WebSocket.Server;

namespace Raid.Service
{
    internal abstract class ApiHandler : IMessageScopeHandler
    {
        private Dictionary<string, EventHandler<SerializableEventArgs>> m_eventHandlerDelegates = new();
        private IReadOnlyDictionary<string, ApiMemberDefinition> m_methods;

        public abstract string Name { get; }

        public ApiHandler()
        {
            m_methods = GetType().GetMembers()
                .Select(member => new ApiMemberDefinition(member, member.GetCustomAttribute<PublicApiAttribute>()))
                .Where(member => member.Attribute != null)
                .ToDictionary(member => member.Attribute.Name ?? member.Name);
        }

        public void HandleMessage(SocketMessage message, WebSocketSession session)
        {
            switch (message.Channel)
            {
                case "call":
                    CallMethod(message.Message.ToObject<CallMethodMessage>(), session);
                    break;
                case "get":
                    GetProperty(message.Message.ToObject<GetPropertyMessage>(), session);
                    break;
                case "sub":
                    Subscribe(message.Message.ToObject<SubscriptionMessage>(), session);
                    break;
                case "unsub":
                    Unsubscribe(message.Message.ToObject<SubscriptionMessage>(), session);
                    break;
            }
        }

        private void Subscribe(SubscriptionMessage subscriptionMessage, WebSocketSession session)
        {
            try
            {
                EventInfo eventInfo = GetPublicApi<EventInfo>(subscriptionMessage.EventName);
                if (!m_eventHandlerDelegates.TryGetValue($"{session.SessionID}:{subscriptionMessage.EventName}", out EventHandler<SerializableEventArgs> handler))
                {
                    handler = (object sender, SerializableEventArgs args) => SendEvent(eventInfo, session, args);
                    m_eventHandlerDelegates.Add($"{session.SessionID}:{subscriptionMessage.EventName}", handler);
                }
                eventInfo.AddEventHandler(this, handler);
            }
            catch (Exception)
            {
                // TODO: Logging
            }
        }

        private async Task SendEvent(EventInfo eventInfo, WebSocketSession session, SerializableEventArgs args)
        {
            if (session.State != SuperSocket.SessionState.Connected)
            {
                if (m_eventHandlerDelegates.Remove($"{session.SessionID}:{args.EventName}", out var handler))
                {
                    eventInfo.RemoveEventHandler(this, handler);
                }
                return;
            }
            SendEventMessage eventMsg = new()
            {
                EventName = args.EventName,
                Payload = JArray.FromObject(args.EventArguments)
            };
            SocketMessage message = new()
            {
                Scope = Name,
                Channel = "send-event",
                Message = JToken.FromObject(eventMsg)
            };
            await session.SendAsync(JsonConvert.SerializeObject(message));
        }

        private void Unsubscribe(SubscriptionMessage subscriptionMessage, WebSocketSession session)
        {
            try
            {
                EventInfo eventInfo = GetPublicApi<EventInfo>(subscriptionMessage.EventName);
                if (!m_eventHandlerDelegates.TryGetValue($"{session.SessionID}:{subscriptionMessage.EventName}", out EventHandler<SerializableEventArgs> handler))
                    return;

                eventInfo.RemoveEventHandler(this, handler);
            }
            catch (Exception)
            {
                // TODO: Logging
            }
        }

        private async void CallMethod(CallMethodMessage message, WebSocketSession session)
        {
            try
            {
                MethodInfo methodInfo = GetPublicApi<MethodInfo>(message.MethodName);

                var methodParameters = methodInfo.GetParameters();
                if (methodParameters.Length < message.Parameters.Count)
                    throw new TargetParameterCountException();

                object[] args = new object[message.Parameters.Count];
                for (int p = 0; p < methodParameters.Length; ++p)
                {
                    if (p >= message.Parameters.Count && !methodParameters[p].IsOptional)
                        throw new TargetParameterCountException();

                    args[p] = message.Parameters[p]?.ToObject(methodParameters[p].ParameterType);
                }

                object result = methodInfo.Invoke(this, args);
                var returnValue = await message.Resolve(result);
                var response = new SocketMessage() { Scope = Name, Channel = "set-promise", Message = returnValue };
                await session.SendAsync(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                // TODO: Logging
                var response = new SocketMessage() { Scope = Name, Channel = "set-promise", Message = message.Reject(ex) };
                await session.SendAsync(JsonConvert.SerializeObject(response));
            }
        }

        private async void GetProperty(GetPropertyMessage message, WebSocketSession session)
        {
            try
            {
                PropertyInfo propertyInfo = GetPublicApi<PropertyInfo>(message.PropertyName);
                object result = propertyInfo.GetValue(this);
                var returnValue = await message.Resolve(result);
                var response = new SocketMessage() { Scope = Name, Channel = "set-promise", Message = returnValue };
                await session.SendAsync(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                // TODO: Logging
                var response = new SocketMessage() { Scope = Name, Channel = "set-promise", Message = message.Reject(ex) };
                await session.SendAsync(JsonConvert.SerializeObject(response));
            }
        }

        private T GetPublicApi<T>(string name) where T : MemberInfo
        {
            if (m_methods.TryGetValue(name, out ApiMemberDefinition member) && member.MemberInfo is T result)
            {
                return result;
            }
            throw new MissingMethodException(Name, name);
        }

        private class ApiMemberDefinition
        {
            public string Name => MemberInfo.Name;
            public MemberInfo MemberInfo { get; }
            public PublicApiAttribute Attribute { get; }

            public ApiMemberDefinition(MemberInfo memberInfo, PublicApiAttribute attribute)
            {
                MemberInfo = memberInfo;
                Attribute = attribute;
            }
        }
    }

}