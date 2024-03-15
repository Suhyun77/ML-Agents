namespace Anipen.AI_NPC_Demo
{
    using UnityEngine;
    using Unity.MLAgents;
    using Unity.MLAgents.Sensors;
    using Unity.MLAgents.Actuators;
    using Unity.Sentis;
    using Random = UnityEngine.Random;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using System;

    public enum AgentMode { Training, Inferencing }

    public class CatAgent : Agent
    {
        #region Members
        [SerializeField] AgentMode agentMode;
        [SerializeField] float moveSpeed = 1f;

        private CatTrainEnvController envController;
        private Animator animator;
        [SerializeField] private int activeTargetCount;
        #endregion

        #region MLAgent
        public override void Initialize()
        {
            Debug.Log("Initialize");
            envController = transform.parent.GetComponent<CatTrainEnvController>();
            envController.InitializeEnv();

            animator = GetComponent<Animator>();
        }
        
        public override void OnEpisodeBegin()
        {
            envController.ResetEnv();
            activeTargetCount = envController.TrainTargetCount;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(transform.localPosition);
            sensor.AddObservation(activeTargetCount);
            foreach(Transform target in envController.TargetList)
            {
                Debug.Log("Observe Target : " + target.gameObject.name);
                sensor.AddObservation(target.localPosition);
            }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            MoveAgent(actions);
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            ActionSegment<float> continuousAction = actionsOut.ContinuousActions;
            continuousAction[0] = Input.GetAxisRaw("Horizontal");
            continuousAction[1] = Input.GetAxisRaw("Vertical");
        }
        #endregion

        #region Trigger
        private void OnTriggerEnter(Collider other)
        {
            switch (agentMode)
            {
                case AgentMode.Training:
                    TrainModeTrigger(other.gameObject);
                    break;
                case AgentMode.Inferencing:
                    InferencingModeTrigger(other.gameObject);
                    break;
            }
        }

        private void TrainModeTrigger(GameObject triggerObj)
        {
            if (triggerObj.CompareTag("Wall"))
            {
                SetReward(-1f);
                EndEpisode();
            }

            if (triggerObj.gameObject.CompareTag("Target"))
            {
                triggerObj.gameObject.SetActive(false);
                activeTargetCount -= 1;
                
                AddReward(1);

                if (activeTargetCount == 0)
                {
                    SetReward(10);
                    EndEpisode();
                }
                    
            }
        }

        private async void InferencingModeTrigger(GameObject triggerObj)
        {
            if (triggerObj.gameObject.CompareTag("Target"))
            {
                triggerObj.gameObject.SetActive(false);
                activeTargetCount -= 1;

                await UniTask.Delay(TimeSpan.FromSeconds(5));
                envController.ActiveRandomTarget();
            }
        }
        #endregion

        #region Method
        private void MoveAgent(ActionBuffers actions)
        {
            var continuousAction = actions.ContinuousActions;

            var dir = new Vector3(continuousAction[0], 0f, continuousAction[1]).normalized;
            transform.localPosition += dir * Time.deltaTime * moveSpeed;
            if (dir.magnitude > 0.5f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 7f);
            }

            string animParam = dir.magnitude > 0.5f ? "Walk" : "Idle";
            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Split('|')[1] != animParam.ToLower())
            {
                animator.SetTrigger(animParam);
            }
            AddReward(-0.001f);
        }
        #endregion
    }
}



