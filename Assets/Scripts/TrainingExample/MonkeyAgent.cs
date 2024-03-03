using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

// Code Monkey
public class MonkeyAgent : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private Material winMat;
    [SerializeField] private Material loseMat;
    [SerializeField] private MeshRenderer floorMeshRenderer;

    // 초기화 함수
    public override void Initialize()
    {
        base.Initialize();
    }

    // 에피소드 시작 시 호출
    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-3, 3), 0.5f, Random.Range(-3, 3));
        target.localPosition = new Vector3(Random.Range(-3, 3), 0.5f, Random.Range(-3, 3));
    }

    // Agent에게 Vector Observation(Behavior Parameter) 전달
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
    }

    // Brain으로부터 Agent가 결정한 행동 전달, 보상 업데이트, 에피소드 종료 등 구현
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 1f;
        transform.localPosition += new Vector3(moveX, 0.5f, moveZ) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousAction = actionsOut.ContinuousActions;
        continuousAction[0] = Input.GetAxisRaw("Horizontal");
        continuousAction[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Target>(out Target target))
        {
            SetReward(+1f);
            floorMeshRenderer.material = winMat;
            EndEpisode();
        }
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-1f);
            floorMeshRenderer.material = loseMat;
            EndEpisode();
        }
    }
}