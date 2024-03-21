namespace Anipen.AI_NPC_Demo
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AI;

    public class MoveController : ActorControllerBase
    {
        //private float turnSmoothVelocity = 1.0f;
        //private float turnSmoothTime = 0.25f;
        //private float moveSpeed = 2f;

        private Transform targetTransform;
        private NavMeshAgent targetAgent;

        public MoveController(NavMeshAgent navMeshAgent)
        {
            this.targetTransform = navMeshAgent.transform;
            this.targetAgent = navMeshAgent;
        }

        public void Move(Vector2 moveVector)
        {
            if (targetTransform == null)
                return;

            // agent.SetDestination

        }
    }
}
