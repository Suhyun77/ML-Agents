using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using TMPro;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.UI;

#region Past Code
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
#endregion

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
    public bool isTouchFinished;
    public Transform manTr;

    private Vector3 dir;

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

        foreach (var target in targetList)
            sensor.AddObservation(target.transform.localPosition);

        sensor.AddObservation(currActiveTargetCount);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (isTargetActive)
            MoveAgent(actions);
        else
        {
            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Split('|')[1] 
        != "sit" && dir.magnitude > 0)
            animator.SetTrigger("Sit");
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
    private async void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Target"))
        {
            agentScore += 1;
            currActiveTargetCount -= 1;
            agentScoreTxt.text = agentScore.ToString();
            other.gameObject.SetActive(false);
            AddReward(1);

            // if (other.gameObject.name == "Target2")
            // {
            //     // GetComponent<SequentialAgent>().enabled = false;
            //     // transform.forward = -manTr.forward;
            //     isTargetActive = false;
            //     // animator.SetTrigger("Sit");
            //     // await UniTask.WaitUntil(() => animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Split('|')[1] == "sit");
            //     // animator.SetTrigger("Sit");
            //     await UniTask.Delay(TimeSpan.FromSeconds(3));
            //     // GetComponent<SequentialAgent>().enabled = true;
            //     isTargetActive = true;

            // }

            if (currActiveTargetCount == 0)
            {
                Debug.Log("End");
                isTargetActive = false;
                await UniTask.Delay(TimeSpan.FromSeconds(5));
                InitTrainEnvironment();
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
        agentScore = 0;
        agentScoreTxt.text = "0";

        var pos = GetRandomGroundPos();
        transform.localPosition = new Vector3(pos.Item1, transform.localPosition.y, pos.Item2);
        transform.localRotation = Quaternion.identity;
    }

    private void InitTrainEnvironment()
    {
        foreach (var target in targetList)
        {
        //     target.gameObject.SetActive(false);
        //     var randPos = GetRandomGroundPos();
        //     target.localPosition = new Vector3(randPos.Item1, target.localPosition.y, randPos.Item2);
            target.gameObject.SetActive(true);
        }
        currActiveTargetCount = activeTargetCount;
        isTargetActive = true;
    }

    private void MoveAgent(ActionBuffers actionBuffers)
    {
        Debug.Log("MoveAgent");
        var continuousAction = actionBuffers.ContinuousActions;

        dir = new Vector3(continuousAction[0], 0f, continuousAction[1]).normalized;
        transform.localPosition += dir * Time.deltaTime * moveSpeed;

        if (dir.magnitude > 0.5f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 7f);
        }
        string animParam = dir.magnitude > 0.5f ? "Walk" : "Idle";
        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Split('|')[1] 
        != animParam.ToLower())
        {
            animator.SetTrigger(animParam);
        }
        AddReward(-0.001f);
    }

    public AudioClip miauSound;
    private void OnMiau()
    {
        AudioSource.PlayClipAtPoint(miauSound, Camera.main.transform.position);
    }
}