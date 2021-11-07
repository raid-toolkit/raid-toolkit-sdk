using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Raid.DataModel;
using Raid.Service.Messages;

namespace Raid.Service
{
    internal abstract class ApiHandler : IMessageScopeHandler
    {
        private Dictionary<string, EventHandler<SerializableEventArgs>> EventHandlerDelegates = new();
        private IReadOnlyDictionary<string, ApiMemberDefinition> Methods;
        protected ILogger<ApiHandler> Logger;

        private readonly string[] SupportedScopes;

        public ApiHandler(ILogger<ApiHandler> logger)
        {
            Logger = logger;

            List<Type> types = new();
            types.Add(GetType());
            types.AddRange(GetType().GetInterfaces());

            Dictionary<string, ApiMemberDefinition> methods = new();
            Methods = methods;

            List<string> supportedScopes = new();

            foreach (Type type in types)
            {
                PublicApiAttribute attr = type.GetCustomAttribute<PublicApiAttribute>(true);
                if (attr == null)
                    continue;

                supportedScopes.Add(attr.Name);
                var members = type.GetMembers()
                    .Select(member => new ApiMemberDefinition(attr.Name, member, member.GetCustomAttribute<PublicApiAttribute>()))
                    .Where(member => member.Attribute != null);
                foreach (var member in members)
                {
                    methods.Add(member.Attribute.Name, member);
                }
            }

            SupportedScopes = supportedScopes.ToArray();
        }

        public bool SupportsScope(string scopeName)
        {
            return SupportedScopes.Contains(scopeName);
        }

        public void HandleMessage(SocketMessage message, ISocketSession session)
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

        private void Subscribe(SubscriptionMessage subscriptionMessage, ISocketSession session)
        {
            try
            {
                EventInfo eventInfo = GetPublicApi<EventInfo>(subscriptionMessage.EventName, out string scope);
                if (!EventHandlerDelegates.TryGetValue($"{session.Id}:{subscriptionMessage.EventName}", out var handler))
                {
                    handler = async (object sender, SerializableEventArgs args) => await SendEvent(eventInfo, session, args, scope);
                    EventHandlerDelegates.Add($"{session.Id}:{subscriptionMessage.EventName}", handler);
                }
                eventInfo.AddEventHandler(this, handler);
            }
            catch (Exception ex)
            {
                Logger.LogError(ServiceError.ApiProxyException.EventId(), ex, "Failed to subscribe");
            }
        }

        private async Task SendEvent(EventInfo eventInfo, ISocketSession session, SerializableEventArgs args, string scope)
        {
            try
            {
                EventHandler<SerializableEventArgs> handler;
                if (!session.Connected)
                {
                    if (EventHandlerDelegates.Remove($"{session.Id}:{args.EventName}", out handler))
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
                    Scope = scope,
                    Channel = "send-event",
                    Message = JToken.FromObject(eventMsg)
                };
                await session.Send(message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ServiceError.ApiProxyException.EventId(), ex, "Failed to send event");
            }
        }

        private void Unsubscribe(SubscriptionMessage subscriptionMessage, ISocketSession session)
        {
            string scope = string.Empty;
            try
            {
                EventInfo eventInfo = GetPublicApi<EventInfo>(subscriptionMessage.EventName, out scope);
                if (!EventHandlerDelegates.TryGetValue($"{session.Id}:{subscriptionMessage.EventName}", out EventHandler<SerializableEventArgs> handler))
                    return;

                eventInfo.RemoveEventHandler(this, handler);
            }
            catch (Exception ex)
            {
                Logger.LogError(ServiceError.ApiProxyException.EventId(), ex, "Failed to unsubscribe");
            }
        }

        private async void CallMethod(CallMethodMessage message, ISocketSession session)
        {
            string scope = string.Empty;
            try
            {
                MethodInfo methodInfo = GetPublicApi<MethodInfo>(message.MethodName, out scope);

                var methodParameters = methodInfo.GetParameters();
                if (methodParameters.Length < message.Parameters.Count)
                    throw new TargetParameterCountException();

                object[] args = new object[methodParameters.Length];
                for (int p = 0; p < methodParameters.Length; ++p)
                {
                    if (p >= message.Parameters.Count)
                    {
                        if (!methodParameters[p].IsOptional)
                            throw new TargetParameterCountException();
                        args[p] = Type.Missing;
                        continue;
                    }

                    args[p] = message.Parameters[p]?.ToObject(methodParameters[p].ParameterType);
                }

                object result = methodInfo.Invoke(this, args);
                var returnValue = await message.Resolve(result);
                var response = new SocketMessage() { Scope = scope, Channel = "set-promise", Message = returnValue };
                await session.Send(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ServiceError.ApiProxyException.EventId(), ex, "Api call failed");
                var response = new SocketMessage() { Scope = scope, Channel = "set-promise", Message = message.Reject(ex) };
                await session.Send(response);
            }
        }

        private async void GetProperty(GetPropertyMessage message, ISocketSession session)
        {
            string scope = string.Empty;
            try
            {
                PropertyInfo propertyInfo = GetPublicApi<PropertyInfo>(message.PropertyName, out scope);
                object result = propertyInfo.GetValue(this);
                var returnValue = await message.Resolve(result);
                var response = new SocketMessage() { Scope = scope, Channel = "set-promise", Message = returnValue };
                await session.Send(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ServiceError.ApiProxyException.EventId(), ex, "Api property access failed");
                var response = new SocketMessage() { Scope = scope, Channel = "set-promise", Message = message.Reject(ex) };
                await session.Send(response);
            }
        }

        private T GetPublicApi<T>(string name, out string scope) where T : MemberInfo
        {
            if (Methods.TryGetValue(name, out ApiMemberDefinition member) && member.MemberInfo is T result)
            {
                scope = member.Scope;
                return result;
            }
            throw new MissingMethodException(member?.Scope ?? "", name);
        }

        private class ApiMemberDefinition
        {
            public string Scope { get; }
            public string Name => MemberInfo.Name;
            public MemberInfo MemberInfo { get; }
            public PublicApiAttribute Attribute { get; }

            public ApiMemberDefinition(string scope, MemberInfo memberInfo, PublicApiAttribute attribute)
            {
                Scope = scope;
                MemberInfo = memberInfo;
                Attribute = attribute;
            }
        }
    }

}