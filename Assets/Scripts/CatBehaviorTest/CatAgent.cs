using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CatAgent : Agent
{
    #region Members
    [SerializeField] Collider groundCol;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float rotSpeed = 1f;

    #endregion

    #region Agent Method
    public override void Initialize()
    {
        
    }

    public override void OnEpisodeBegin()
    {
        var extentsX = groundCol.bounds.extents.x;
        var extentsZ = groundCol.bounds.extents.z;
        var randomRangeX = Random.Range(-extentsX, extentsX);
        var randomRangeZ = Random.Range(-extentsZ, extentsZ);

        transform.localPosition = new Vector3(randomRangeX, transform.localPosition.y, randomRangeZ);
        transform.localRotation = Quaternion.identity;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // continuous action
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        float rotY = actions.ContinuousActions[2];

        transform.localPosition += new Vector3(moveX, transform.localPosition.y, moveZ) * Time.deltaTime * moveSpeed;
        transform.localRotation *= Quaternion.Euler(0f, rotY * rotSpeed, 0f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousAction = actionsOut.ContinuousActions;
        continuousAction[0] = Input.GetAxisRaw("Horizontal");
        continuousAction[1] = Input.GetAxisRaw("Vertical");
        //continuousAction[2] = 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            SetReward(-1f);
            EndEpisode();
        }
        else
        {
            SetReward(+1f);
            EndEpisode();
        }
    }
    #endregion
}
