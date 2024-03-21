namespace Anipen.AI_NPC_Demo
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class AnimationController : ActorControllerBase
    {
        private Animator animator;

        public AnimationController(Animator animator)
        {
            this.animator = animator;
        }

        public void ExecuteAnimationCommand(AnimationCommand animationCommand)
        {
            animator.Play(animationCommand.AnimationState.ToString());
        }
    }
}
