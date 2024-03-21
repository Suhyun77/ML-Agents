namespace Anipen
{
    using System.Collections.Generic;

    public class StateBroadCaster<T> : List<IStateObserver<T>>, IStateSubject<T> where T : System.Enum
    {
        #region Implementation : IStateSubject
        public void AddObserver(IStateObserver<T> observer)
        {
            if (!Contains(observer))
                Add(observer);
        }

        public void RemoveObserver(IStateObserver<T> observer)
        {
            Remove(observer);
        }

        public void NotifyChangeState(T state)
        {
            foreach (var observer in this)
                observer.Notify(state);
        }
        #endregion
    }
}
