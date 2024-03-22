namespace Anipen.AI_NPC_Demo
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AI;

    public class Actor : MonoBehaviour
    {
        #region Members
        [SerializeField] Transform modelRoot;

        private Animator animator;
        private NavMeshAgent navMeshAgent;

        #region Controllers
        private PatternController patternController;
        private AnimationController animController;
        private SpeechController speechController;
        private MoveController moveController;
        #endregion

        //private ActorType type;
        #endregion

        private void Start()
        {
            // test
            //type = ActorType.Player;  

            Init();
        }

        private void Init()
        {
            patternController = new PatternController();
            animController = new AnimationController(animator);
            speechController = new SpeechController();
            moveController = new MoveController(navMeshAgent);
        }
    }
}
