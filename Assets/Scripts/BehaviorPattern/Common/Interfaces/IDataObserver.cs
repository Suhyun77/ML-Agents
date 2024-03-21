namespace Anipen
{
    public interface IDataObserver<T> where T : new()
    {
        public void Notify(T data);
    }

    public interface IDataSubject<T> where T : new()
    {
        public void AddObserver(IDataObserver<T> observer);
        public void RemoveObserver(IDataObserver<T> observer);
        public void NotifyObservers(T data);
    }
}