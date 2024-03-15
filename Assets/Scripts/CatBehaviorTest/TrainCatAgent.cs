using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using TMPro;
using Cysharp.Threading.Tasks;
using System;

public class TrainCatAgent : Agent
{
    #region Members
    [SerializeField] int trainTargetIdx;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] Collider groundCol;
    [SerializeField] Transform targetRoot;
    [SerializeField] List<Transform> targetList;
    [SerializeField] Transform gateRoot;
    [SerializeField] List<Transform> gateList;
    [SerializeField] TMP_Text agentScoreTxt;

    public Animator animator;
    public int agentScore;
    public bool isTargetActive;
    public int currActiveTargetCount;

    private int currTargetIdx;
    #endregion

    #region MLAgent
    public override void Initialize()
    {
        animator = GetComponent<Animator>();

        targetList = new List<Transform>
        {
            targetRoot.GetChild(0),
            targetRoot.GetChild(2)
        };
    }

    public override void OnEpisodeBegin()
    {
        InitPlayer();
        InitTrainEnvironment();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);

        sensor.AddObservation(targetList[0].localPosition);

        sensor.AddObservation(currActiveTargetCount);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
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

    #region Trigger
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter : " + other.gameObject.name);

        if (other.gameObject.CompareTag("Wall"))
        {
            SetReward(-1f);
            EndEpisode();
        }

        if (other.gameObject.CompareTag("Target"))
        {
            agentScore += 1;
            currActiveTargetCount -= 1;
            agentScoreTxt.text = agentScore.ToString();
            other.gameObject.SetActive(false);
            AddReward(1);

            if (currActiveTargetCount == 0)
            {
                Debug.Log("Target : " + other.gameObject.name);
                isTargetActive = false;

                // For Train
                EndEpisode();

                // For Apply Model
                //await UniTask.Delay(TimeSpan.FromSeconds(5));
                //InitTrainEnvironment();
            }
        }
    }
    #endregion

    #region Method
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
        agentScore = 0;
        agentScoreTxt.text = "0";

        //var pos = GetRandomGroundPos();
        //transform.localPosition = new Vector3(pos.Item1, transform.localPosition.y, pos.Item2);
        transform.localPosition = new Vector3(0, transform.localPosition.y, 0.3f);
        transform.localRotation = Quaternion.identity;
    }

    private void InitTrainEnvironment()
    {
        //var randPos = GetRandomGroundPos();
        //targetList[0].transform.localPosition = new Vector3(randPos.Item1, targetList[0].transform.localPosition.y, targetList[0].transform.localPosition.z);

        foreach (var target in targetList)
            target.gameObject.SetActive(false);

        currTargetIdx = Random.Range(0, 2);
        targetList[currTargetIdx].gameObject.SetActive(true);
        currActiveTargetCount = 1;  // *
        isTargetActive = true;
    }

    private void MoveAgent(ActionBuffers actionBuffers)
    {
        var continuousAction = actionBuffers.ContinuousActions;

        var dir = new Vector3(continuousAction[0], 0f, continuousAction[1]).normalized;
        transform.localPosition += dir * Time.deltaTime * moveSpeed;

        if (dir.magnitude > 0.5f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 7f);
        }

        string animParam = dir.magnitude > 0.5f ? "Walk" : "Idle";
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animParam))
        {
            animator.SetTrigger(animParam);
        }
        AddReward(-0.001f);
    }
    #endregion
}
