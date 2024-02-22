using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RollerAgent : Agent
{
    #region Members
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private Transform target;
    [SerializeField] private float forceMultiplier = 10;
    #endregion

    #region Agent Method
    // Agents가 임무 수행(target에 도달)할 때까지 episode들이 실행됨
    // OnEpisodeBegin : new episode를 위한 환경 설정 (episode 초기화, reset train)
    public override void OnEpisodeBegin()
    {
        // Agent Fell -> momentum = 0
        if (transform.localPosition.y < 0)
        {
            rigidBody.angularVelocity = Vector3.zero;
            rigidBody.velocity = Vector3.zero;
            //transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        // set random position
        // 하나의 position만 학습하면 이후에 target 위치가 바뀌면 target을 찾지 못함 따라서 random한 위치로 다양하게 학습시켜야 함
        transform.localPosition = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
        target.localPosition = new Vector3(Random.value *8-4, 0.5f, Random.value *8-4);
    }

    // Agent가 모은 정보(feature Vector) -> Brain
    // Agent가 학습하려면 Agent가 임무를 완수하기 위해 필요한 정보를 모두 AddObservation 해야함
    public override void CollectObservations(VectorSensor sensor)
    {
        // Add Observation target
        sensor.AddObservation(target.localPosition);  // Vector Observation = 3
        sensor.AddObservation(transform.localPosition);  // Vector Observation = 6

        // Agent Velocity
        sensor.AddObservation(rigidBody.velocity.x);  // Vector Observation = 7
        sensor.AddObservation(rigidBody.velocity.z);  // Vector Observation = 8
    }

    // Agent가 액션에 대한 보상을 결정하는 메소드
    // 보상 획득 후, 에피소드를 종료하는 분기를 설정할 수 있음
    public override void OnActionReceived(ActionBuffers actions)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];
        rigidBody.AddForce(controlSignal * forceMultiplier);

        // Rewards
        float distanceToTarget = Vector3.Distance(transform.localPosition, target.localPosition);
        // Reached target
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }
        // Fell off platform
        else if (transform.localPosition.y < 0)
        {
            SetReward(-3.0f);
            EndEpisode();
        }
    }
    #endregion
}
