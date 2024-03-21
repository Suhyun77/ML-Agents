namespace Anipen
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    using Cysharp.Threading.Tasks;

    public class SceneComposer : SingletonGameObject<SceneComposer>
    {
        #region Members
        private List<ISceneComponent> sceneComponents = new List<ISceneComponent>();
        #endregion

        #region Properties
        public bool IsComposed { get; protected set; }
        #endregion

        #region Mono
        private void Start()
        {
            ComposeScene().Forget();
        }
        #endregion

        #region Methods
        public void AddSceneComponent(ISceneComponent comp)
        {
            if(!sceneComponents.Contains(comp))
                sceneComponents.Add(comp);
        }

        public virtual async UniTaskVoid ComposeScene()
        {
            List<ISceneComponent> temp = new List<ISceneComponent>(sceneComponents);

            foreach (var component in sceneComponents)
                component.Compose(() => temp.Remove(component));

            await UniTask.WaitUntil(() => temp.Count == 0);

            IsComposed = true;
        }
        #endregion
    }
}
