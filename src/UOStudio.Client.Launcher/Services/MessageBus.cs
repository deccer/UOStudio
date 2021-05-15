using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace UOStudio.Client.Launcher.Services
{
    public sealed class MessageBus : IMessageBus
    {
        private interface ISubscriber { }

        public class Subscriber<TMessage> : ISubscriber
        {
            private readonly Func<TMessage, Task> _handler;

            internal Subscriber(Func<TMessage, Task> handler)
            {
                _handler = handler;
            }

            public static Task PublishAsync(Subscriber<TMessage> subscriber, TMessage message)
                => subscriber._handler.Invoke(message);

            public static ConfiguredTaskAwaitable PublishConfiguredAsync(Subscriber<TMessage> subscriber, TMessage message)
                => subscriber._handler.Invoke(message)
                    .ConfigureAwait(false);

            public static void Publish(Subscriber<TMessage> subscriber, TMessage message)
                => subscriber._handler.Invoke(message)
                    .ContinueWith(_ => _)
                    .ConfigureAwait(false);
        }

        public readonly struct SubscriptionToken
        {
            private Guid Value { get; }

            internal Type MessageType { get; }

            public SubscriptionToken(Type messageType, Guid value)
            {
                MessageType = messageType;
                Value = value;
            }

            public static implicit operator Guid(SubscriptionToken subscriptionToken)
            {
                return subscriptionToken.Value;
            }
        }

        private readonly ConcurrentDictionary<Type, HashSet<(Guid Token, ISubscriber Subscriber)>> _subscriptions = new();

        public SubscriptionToken Subscribe<TMessage>(Func<TMessage, Task> subscriber)
        {
            var messageType = typeof(TMessage);
            if (!_subscriptions.ContainsKey(messageType))
            {
                _subscriptions[messageType] = new HashSet<(Guid, ISubscriber)>();
            }

            var token = Guid.NewGuid();
            _subscriptions[messageType].Add((token, new Subscriber<TMessage>(subscriber)));
            return new SubscriptionToken(messageType, token);
        }

        public void Unsubscribe(SubscriptionToken subscriptionToken)
        {
            if (_subscriptions.TryGetValue(subscriptionToken.MessageType, out var subscribers))
            {
                subscribers.RemoveWhere(tuple => tuple.Token == subscriptionToken);
            }
        }

        public async Task PublishWaitAllAsync<TMessage>(TMessage message)
        {
            if (_subscriptions.TryGetValue(typeof(TMessage), out var subscribers))
            {
                var publishedTasks = new List<Task>();

                foreach (var subscriber in subscribers)
                {
                    if (subscriber.Subscriber is Subscriber<TMessage> validSubscriber)
                    {
                        var publishTask = Subscriber<TMessage>.PublishAsync(validSubscriber, message);
                        publishedTasks.Add(publishTask);
                    }
                }

                await Task.WhenAll(publishedTasks);
            }
        }

        public async Task PublishWaitAsync<TMessage>(TMessage message)
        {
            if (_subscriptions.TryGetValue(typeof(TMessage), out var subscribers))
            {
                foreach (var subscriber in subscribers)
                {
                    if (subscriber.Subscriber is Subscriber<TMessage> validSubscriber)
                    {
                        await Subscriber<TMessage>.PublishConfiguredAsync(validSubscriber, message);
                    }
                }
            }
        }

        public void PublishWait<TMessage>(TMessage message)
        {
            if (!_subscriptions.TryGetValue(typeof(TMessage), out var subscribers))
            {
                return;
            }

            foreach (var subscriber in subscribers)
            {
                if (subscriber.Subscriber is Subscriber<TMessage> validSubscriber)
                {
                    Subscriber<TMessage>.Publish(validSubscriber, message);
                }
            }
        }
    }
}
