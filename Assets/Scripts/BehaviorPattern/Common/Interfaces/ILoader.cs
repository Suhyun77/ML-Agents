namespace Anipen
{
    public interface ILoader
    {
        public bool isLoadSuccess { get; set; }
        public void StartLoad(System.Action<ILoader> finishCallback);
    }
}
