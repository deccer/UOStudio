using System;
using System.Threading.Tasks;

namespace UOStudio.Client.Launcher.Services
{
    public interface IMessageBus
    {
        MessageBus.SubscriptionToken Subscribe<TMessage>(Func<TMessage, Task> subscriber);
        void Unsubscribe(MessageBus.SubscriptionToken subscriptionToken);
        Task PublishWaitAllAsync<TMessage>(TMessage message);
        Task PublishWaitAsync<TMessage>(TMessage message);
        void PublishWait<TMessage>(TMessage message);
    }
}