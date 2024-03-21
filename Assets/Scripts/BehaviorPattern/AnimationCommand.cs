namespace Anipen.AI_NPC_Demo
{
    using System;
    using System.Collections.Generic;

    #region Test Character Animation
    public enum CharacterAnimationState
    {
        None, Idle, Walk,

        // Blend Animation
        Sit_Idle_01, Idle_01, Cellphone_Idle, Cellphone_Idle_Soft,

        // Social Action
        Hi, Regret, Cheer, Clap, Fighting, Laugh, Lookaround, Sit, StandUp, Beer,
        Selfie, Throw, Sofa, Surprise, Worry, Point, Sofa_StandUp, Sport_Sit, Sport_StandUp,
        Basketball, Roullet, Standup_Hand, Pubcleck_Hi, Streetstall,

        // Stage
        Stage_Sit, Stage, Streetvendor,

        // BasketBall Action
        Basketball_Ready, Basketball_Shoot,

        // Interactive Action
        Friendship_A,
        Friendship_B,
        Hug_A,
        Hug_B,
        Highfive_A,
        Highfive_B,
        Talk_A,
        Talk_B,
        Selfie_Success_A,
        Selfie_Success_B,

        // ETC
        Cellphone_A,
        Cellphone_B,
    }
    #endregion

    public class AnimationCommand : Command
    {
        #region Member
        private CharacterAnimationState animationState = CharacterAnimationState.None;
        #endregion

        #region Action
        private List<TimeTriggerEvent> timeTriggerEvents = new List<TimeTriggerEvent>();
        private string targetAvatarID;
        #endregion

        #region Property
        public Dictionary<string, object> dataDic { get; protected set; } = new Dictionary<string, object>();
        public CharacterAnimationState AnimationState => animationState;
        public string AnimationTargetAvatarID => targetAvatarID;
        public List<TimeTriggerEvent> TimeTriggerEvents => timeTriggerEvents;
        #endregion

        #region Constructor
        public AnimationCommand(CharacterAnimationState animationName, string targetAvatarID)
        {
            excutableType = CommandType.Animation;
        }
        #endregion
    }

    #region Local Class : Time Trigger
    public class TimeTriggerEvent
    {
        public float playTime = -1.0f;
        public bool isMustPlay = false;
        Action action = default;

        public TimeTriggerEvent(float time, Action action, bool isMustPlay)
        {
            this.playTime = time;
            this.action = action;
            this.isMustPlay = isMustPlay;
        }

        public void Do() => action?.Invoke();
    }
    #endregion
}