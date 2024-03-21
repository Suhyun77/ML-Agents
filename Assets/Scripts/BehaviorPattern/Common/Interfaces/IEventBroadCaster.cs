namespace Anipen
{
    using System;

    public interface IEventBroadCaster<T> where T : EventArgs
    {

        void RegisterEvent(EventHandler<T> handler);
        void UnRegisterEvent(EventHandler<T> handler);
        void RaiseEvent(T evt);
    }
}
