using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MonkeyAgent : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private Material winMat;
    [SerializeField] private Material loseMat;
    [SerializeField] private Material defaultMat;
    [SerializeField] private MeshRenderer floorMeshRenderer;


    public override void OnEpisodeBegin()
    {
        Debug.Log("OnEpisodeBegin");
        transform.localPosition = new Vector3(Random.Range(-3, 3), 0.5f, Random.Range(-3, 3));
        target.localPosition = new Vector3(Random.Range(-3, 3), 0.5f, Random.Range(-3, 3));

        floorMeshRenderer.material = defaultMat;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Debug.Log("CollectObservations");
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Debug.Log("OnActionReceived");
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 1f;
        transform.localPosition += new Vector3(moveX, 0.5f, moveZ) * Time.deltaTime * moveSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter : " + other.gameObject.name);
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