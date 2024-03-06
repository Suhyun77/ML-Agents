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
    public int agentScore;
    public bool isTargetActive;
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
        InitPlayer();
        InitTrainEnvironment();

        //transform.localPosition = new Vector3(0, 0.066f, 0);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);

        foreach (var target in targetList)
            sensor.AddObservation(target.transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (isTargetActive)
            MoveAgent(actions);
        else
            animator.SetTrigger("Sit");

        if (agentScore == targetList.Count)
        {
            Debug.Log("Finish!");
            SetReward(1);
            EndEpisode();
        }
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
            //var idx = targetList.IndexOf(other.transform);

            //targetList[idx].gameObject.SetActive(false);
            //if (idx < targetList.Count - 1)
            //    targetList[idx + 1].gameObject.SetActive(true);

            agentScore += 1;
            agentScoreTxt.text = agentScore.ToString();

            //if (idx == targetList.Count - 1)
            //{
            //    SetReward(+1f);
            //    EndEpisode();  // for train
            //}

            other.gameObject.SetActive(false);
            isTargetActive = false;

            SetRandomTarget();
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
        agentScore = 0;
        agentScoreTxt.text = "0";

        var pos = GetRandomGroundPos();
        transform.localPosition = new Vector3(pos.Item1, transform.localPosition.y, pos.Item2);
        transform.localRotation = Quaternion.identity;
    }

    bool isRightPos;
    private void InitTrainEnvironment()
    {
        foreach (var target in targetList)
            target.gameObject.SetActive(false);

        SetRandomTarget();

        //isRightPos = false;
        //while (true)
        //{
        //    var targetCol = GetTargetRandomPos();

        //    foreach (var ob in obstacles)
        //    {
        //        bool isIntersect = ob.bounds.Intersects(targetCol.bounds) ? false : true;
        //        isRightPos = 
        //    }

        //    if (isRightPos)

        //}
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

    private void SetRandomTarget()
    {
        var randIdx = Random.Range(0, targetList.Count);
        var randPos = GetRandomGroundPos();
        targetList[randIdx].localPosition = new Vector3(randPos.Item1, targetList[randIdx].localPosition.y, randPos.Item2);
        targetList[randIdx].gameObject.SetActive(true);
        isTargetActive = true;
    }
}
