namespace Anipen.Multiplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    //using Fusion;
    //public class SingletonNetworkBehaviour<T> : NetworkBehaviour where T : NetworkBehaviour
    //{
    //    #region Static
    //    protected static T instance;
    //    public static T Instance
    //    {
    //        get
    //        {
    //            instance = FindObjectOfType(typeof(T)) as T;

    //            if (instance == null)
    //                instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();

    //            return instance;
    //        }
    //    }
    //    #endregion

    //}

    //public class ManagerWithNetworkBase<T> : SingletonNetworkBehaviour<T>, IManager where T : NetworkBehaviour
    //{
     
    //    #region Implementation : IManager
    //    public bool IsInit { get; protected set; }

    //    public virtual void Init()
    //    {
    //        IsInit = true;
    //    }
    //    public virtual void Reset() { }
    //    #endregion
    //}
}
