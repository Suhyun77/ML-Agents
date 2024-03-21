namespace Anipen
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    using Cysharp.Threading.Tasks;

    public class LoadingController : MonoBehaviour
    {
        #region Members
        private List<ILoader> elementsToLoad;
        #endregion

        #region Properties
        public object LoadingData { get; set; }
        #endregion

        #region Constructor
        public LoadingController() { elementsToLoad = new List<ILoader>(); } 
        public LoadingController(object data) { LoadingData = data; }
        #endregion

        #region Methods
        public virtual void RegisterLoader(ILoader loader)
        {
            if(!elementsToLoad.Contains(loader))
                elementsToLoad.Add(loader);
        }

        public async void StartLoading(Action finishCallback = null)
        {
            foreach (var loader in elementsToLoad)
                loader.StartLoad(OnLoaderComplete);

            void OnLoaderComplete(ILoader finishedLoader)
            {
                if (finishedLoader.isLoadSuccess)
                    elementsToLoad.Remove(finishedLoader);
                else
                    Debug.LogFormat("Loader {0} is finished with error", finishedLoader);
            }

            await UniTask.WaitUntil(() => elementsToLoad.Count == 0);

            LoadingData = null;
            finishCallback?.Invoke();
        }
        #endregion
    }
}
