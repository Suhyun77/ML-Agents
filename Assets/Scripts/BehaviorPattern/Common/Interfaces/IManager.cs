namespace Anipen
{
    public interface IManager
    {
        public bool IsInit { get; }

        public void Init();
        public void Reset();
    }
}
