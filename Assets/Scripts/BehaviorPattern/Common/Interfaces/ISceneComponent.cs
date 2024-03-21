namespace Anipen
{
    using System;
    public interface ISceneComponent
    {
        public void Compose(Action finishCallback);
    }
}
