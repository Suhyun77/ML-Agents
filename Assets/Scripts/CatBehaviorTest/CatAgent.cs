using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using TMPro;

public class CatAgent : Agent
{
    #region Members
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] Collider groundCol;
    [SerializeField] Transform targetRoot;
    [SerializeField] List<Transform> targetList;
    [SerializeField] Transform gateRoot;
    [SerializeField] List<Transform> gateList;
    [SerializeField] TMP_Text agentScoreTxt;

    public Animator animator;
    private int agentScore;
    private bool isTargetActive;
    #endregion

    #region Agent Method
    public override void Initialize()
    {
        animator = GetComponent<Animator>();

        targetList = new List<Transform>();
        foreach (Transform target in targetRoot)
        {
            if (target.gameObject.activeSelf)
                targetList.Add(target);
        }
    }

    public override void OnEpisodeBegin()
    {
        agentScore = 0;
        agentScoreTxt.text = "0";

        InitPlayer();
        InitTrainEnvironment();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);

        foreach (var target in targetList)
            sensor.AddObservation(target.transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        isTargetActive = targetList[0].gameObject.activeSelf;

        if (isTargetActive)
            MoveAgent(actions);
        else
            animator.SetTrigger("Sit");

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousAction = actionsOut.ContinuousActions;
        continuousAction[0] = Input.GetAxisRaw("Horizontal");
        continuousAction[1] = Input.GetAxisRaw("Vertical");
    }
    #endregion

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            SetReward(-1f);
            EndEpisode();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Target"))
        {
            var idx = targetList.IndexOf(other.transform);

            targetList[idx].gameObject.SetActive(false);
            if (idx < targetList.Count-1)
                targetList[idx + 1].gameObject.SetActive(true);

            agentScore += 1;
            agentScoreTxt.text = agentScore.ToString();

            if (idx == targetList.Count - 1)
            {
                SetReward(+1f);
                //EndEpisode();
            }
        }
    }

    private (float, float) GetRandomGroundPos()
    {
        var extentsX = groundCol.bounds.extents.x - 0.5f;
        var extentsZ = groundCol.bounds.extents.z - 0.5f;
        var randomRangeX = Random.Range(-extentsX, extentsX);
        var randomRangeZ = Random.Range(-extentsZ, extentsZ);

        return (randomRangeX, randomRangeZ);
    }

    private void InitPlayer()
    {
        var pos = GetRandomGroundPos();

        transform.localPosition = new Vector3(pos.Item1, transform.localPosition.y, pos.Item2);
        transform.localRotation = Quaternion.identity;
    }

    private void InitTrainEnvironment()
    {
        foreach (var target in targetList)
        {
            var randPos = GetRandomGroundPos();
            target.localPosition = new Vector3(randPos.Item1, target.localPosition.y, randPos.Item2);
            target.gameObject.SetActive(false);
        }
        targetList[0].gameObject.SetActive(true);
    }

    private void MoveAgent(ActionBuffers actionBuffers)
    {
        var continuousAction = actionBuffers.ContinuousActions;

        var dir = new Vector3(continuousAction[0], 0f, continuousAction[1]).normalized;
        transform.localPosition += dir * Time.deltaTime * moveSpeed;

        if (dir.magnitude > 0f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 7f);
        }

        string animParam = dir.magnitude == 1 ? "Walk" : "Idle";
        animator.SetTrigger(animParam);

        AddReward(-0.001f);
    }
}
