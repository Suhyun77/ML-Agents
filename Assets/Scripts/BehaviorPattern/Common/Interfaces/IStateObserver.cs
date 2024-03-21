namespace Anipen
{
    public interface IStateObserver<T> where T : System.Enum
    {
        public void Notify(T state);
    }

    public interface IStateSubject<T> where T : System.Enum
    {
        public void AddObserver(IStateObserver<T> observer);
        public void RemoveObserver(IStateObserver<T> observer);
        public void NotifyChangeState(T state);
    }
}