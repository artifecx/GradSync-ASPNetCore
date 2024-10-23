using System;

namespace Services.EventBus
{
    public interface IEventBus
    {
        void Publish<T>(T eventItem);
        void Subscribe<T>(Action<T> handler);
    }

}
