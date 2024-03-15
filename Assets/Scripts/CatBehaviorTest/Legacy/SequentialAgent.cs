using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using TMPro;
using Cysharp.Threading.Tasks;
using System;

public enum TargetState { None, target1, target2 }

//public class SequentialAgent : Agent
//{
//    #region Members
//    [SerializeField] int activeTargetCount;
//    [SerializeField] float moveSpeed = 1f;
//    [SerializeField] Collider groundCol;
//    [SerializeField] Transform targetRoot;
//    [SerializeField] List<Transform> targetList;
//    [SerializeField] Transform gateRoot;
//    [SerializeField] List<Transform> gateList;
//    [SerializeField] TMP_Text agentScoreTxt;

//    public Animator animator;
//    public int agentScore;
//    public bool isTargetActive;

//    //private TargetState currTarget;
//    #endregion

//    #region Agent Method
//    public override void Initialize()
//    {
//        animator = GetComponent<Animator>();

//        targetList = new List<Transform>();
//        for (int i = 0; i < activeTargetCount; i++)
//        {
//            targetList.Add(targetRoot.GetChild(i));
//        }
//    }

//    public override void OnEpisodeBegin()
//    {
//        InitPlayer();
//        InitTrainEnvironment();
//    }

//    public override void CollectObservations(VectorSensor sensor)
//    {
//        sensor.AddObservation(transform.localPosition);

//        foreach (var target in targetList)
//            sensor.AddObservation(target.transform.localPosition);
//    }

//    public override void OnActionReceived(ActionBuffers actions)
//    {
//        if (isTargetActive)
//            MoveAgent(actions);
//        else
//            animator.SetTrigger("Sit");
//    }

//    public override void Heuristic(in ActionBuffers actionsOut)
//    {
//        ActionSegment<float> continuousAction = actionsOut.ContinuousActions;
//        continuousAction[0] = Input.GetAxisRaw("Horizontal");
//        continuousAction[1] = Input.GetAxisRaw("Vertical");
//    }
//    #endregion

//    private void OnCollisionEnter(Collision other)
//    {
//        if (other.gameObject.CompareTag("Wall"))
//        {
//            SetReward(-1f);
//            EndEpisode();
//        }
//    }
//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.gameObject.CompareTag("Target"))
//        {
//            agentScore += 1;
//            agentScoreTxt.text = agentScore.ToString();
//            other.gameObject.SetActive(false);

//            if (agentScore == 2)
//            {
//                SetReward(1);
//                EndEpisode();
//            }
//        }
//    }

//    private (float, float) GetRandomGroundPos()
//    {
//        var extentsX = groundCol.bounds.extents.x - 0.5f;
//        var extentsZ = groundCol.bounds.extents.z - 0.5f;
//        var randomRangeX = Random.Range(-extentsX, extentsX);
//        var randomRangeZ = Random.Range(-extentsZ, extentsZ);

//        return (randomRangeX, randomRangeZ);
//    }

//    private void InitPlayer()
//    {
//        agentScore = 0;
//        agentScoreTxt.text = "0";

//        var pos = GetRandomGroundPos();
//        transform.localPosition = new Vector3(pos.Item1, transform.localPosition.y, pos.Item2);
//        transform.localRotation = Quaternion.identity;
//    }

//    private void InitTrainEnvironment()
//    {
//        foreach (var target in targetList)
//        {
//            target.gameObject.SetActive(false);
//            var randPos = GetRandomGroundPos();
//            target.localPosition = new Vector3(randPos.Item1, target.localPosition.y, randPos.Item2);
//            target.gameObject.SetActive(true);
//        }
//        isTargetActive = true;
//    }

//    private void MoveAgent(ActionBuffers actionBuffers)
//    {
//        var continuousAction = actionBuffers.ContinuousActions;

//        var dir = new Vector3(continuousAction[0], 0f, continuousAction[1]).normalized;
//        transform.localPosition += dir * Time.deltaTime * moveSpeed;

//        if (dir.magnitude > 0)
//        {
//            Quaternion targetRot = Quaternion.LookRotation(dir);
//            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 7f);
//        }

//        string animParam = dir.magnitude == 1 ? "Walk" : "Idle";
//        animator.SetTrigger(animParam);

//        AddReward(-0.001f);
//    }
//}


public class SequentialAgent : Agent
{
    #region Members
    [SerializeField] int activeTargetCount;
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

    private Transform currTarget;

    #endregion

    #region Agent Method
    public override void Initialize()
    {
        animator = GetComponent<Animator>();

        targetList = new List<Transform>();
        for (int i = 0; i < activeTargetCount; i++)
        {
            targetList.Add(targetRoot.GetChild(i));
        }
    }

    public override void OnEpisodeBegin()
    {
        InitPlayer();
        InitTrainEnvironment();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);

        //foreach (var target in targetList)
        //    sensor.AddObservation(target.transform.localPosition);

        sensor.AddObservation(currTarget.localPosition);

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

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            SetReward(-1f);
            EndEpisode();
        }
    }
    private async void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Target"))
        {
            agentScore += 1;
            currActiveTargetCount -= 1;
            agentScoreTxt.text = agentScore.ToString();
            other.gameObject.SetActive(false);
            AddReward(1);

            //if (agentScore == 1)
            //{
                Debug.Log("Target : " + other.gameObject.name);
                isTargetActive = false;

                // For Train
                //EndEpisode();

                // For Apply Model
                await UniTask.Delay(TimeSpan.FromSeconds(10));
                InitTrainEnvironment();
            //}
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

    private void InitTrainEnvironment()
    {
        //var randIdx = Random.Range(0, activeTargetCount);
        //var randPos = GetRandomGroundPos();
        //targetList[randIdx].transform.localPosition = new Vector3(randPos.Item1, targetList[randIdx].transform.localPosition.y, randPos.Item2);
        currTarget = targetList[2].transform;
        currTarget.gameObject.SetActive(true);

        //foreach (var target in targetList)
        //{
        //    target.gameObject.SetActive(false);
        //    var randPos = GetRandomGroundPos();
        //    target.localPosition = new Vector3(randPos.Item1, target.localPosition.y, randPos.Item2);
        //    //target.gameObject.SetActive(true);
        //}

        currActiveTargetCount = activeTargetCount;
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
        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Split('|')[1] != animParam.ToLower())
        {
            animator.SetTrigger(animParam);
        }
        AddReward(-0.001f);
    }
}