namespace Anipen.AI_NPC_Demo
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ActorControllerBase : MonoBehaviour
    {
        public bool IsInit { get; protected set; } = false;

        public virtual void Init()
        {
            IsInit = true;
        }

        public virtual void Reset()
        {
            IsInit = false;
        }
    }
}
