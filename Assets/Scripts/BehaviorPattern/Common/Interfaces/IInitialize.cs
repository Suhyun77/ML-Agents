namespace Anipen
{
    using Cysharp.Threading.Tasks;

    public interface IInitialize
    {
        public UniTaskVoid Initialize();
    }
}
