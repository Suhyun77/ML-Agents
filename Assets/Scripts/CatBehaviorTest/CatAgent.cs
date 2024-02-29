using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine.UI;

public class CatAgent : Agent
{
    #region Members
    [SerializeField] Collider groundCol;
    [SerializeField] MeshRenderer groundMeshRenderer;
    [SerializeField] Transform targetRoot;
    [SerializeField] List<Transform> targetList;
    [SerializeField] List<Material> targetMaterials;
    [SerializeField] Transform gateRoot;
    [SerializeField] List<Transform> gateList;

    [SerializeField] Button feedBtn;
    [SerializeField] GameObject feedObj;

    private Animator animator;
    #endregion


    #region Agent Method
    public override void Initialize()
    {
        animator = GetComponentInChildren<Animator>();
        targetList = new List<Transform>();
        foreach(Transform target in targetRoot)
        {
            if (target.gameObject.activeSelf)
                targetList.Add(target);
        }
        targetMaterials = new List<Material>();
        foreach (var target in targetList)
        {
            var mat = target.GetComponent<MeshRenderer>().material;
            targetMaterials.Add(mat);
        }
        foreach (Transform gate in gateRoot)
        {
            gateList.Add(gate);
        }
        feedBtn.onClick.AddListener(() => feedObj.SetActive(true));
    }

    public override void OnEpisodeBegin()
    {
        keyState = 0;

        InitPlayer();
        InitTrainEnvironment();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);

        foreach(var target in targetList)
            sensor.AddObservation(target.transform.localPosition);
    }


    public int keyState;
    public override void OnActionReceived(ActionBuffers actions)
    {
        MoveAgent(actions);

        switch (keyState)
        {
            case 0:
                {
                    if (Vector3.Distance(targetList[0].transform.localPosition, transform.localPosition) <= 0.5f)
                    {
                        targetList[0].gameObject.SetActive(false);
                        gateList[0].gameObject.SetActive(false);
                        keyState = 1;
                    }
                }
                break;
            case 1:
                {
                    if (Vector3.Distance(targetList[1].transform.localPosition, transform.localPosition) <= 0.5f)
                    {
                        targetList[1].gameObject.SetActive(false);
                        gateList[1].gameObject.SetActive(false);
                        keyState = 2;
                    }
                }
                break;
            case 2:
                {
                    if (Vector3.Distance(targetList[2].transform.localPosition, transform.localPosition) <= 0.5f)
                    {
                        SetReward(1f);
                        EndEpisode();
                    }
                }
                break;
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
            AddReward(-1f);
            EndEpisode();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //if (targetList.Contains(other.transform))
        //{
        //    var idx = targetList.IndexOf(other.transform);

        //    if (other.transform.Equals(targetList[idx]))
        //    {
        //        AddReward(idx+1);
        //        groundMeshRenderer.material = targetMaterials[idx];
        //        //other.transform.gameObject.SetActive(false);

        //        if (idx+1 == 3)
        //        {
        //            SetReward(+1);
        //            EndEpisode();
        //        }

        //    }
        //}

        //if (targetList.Contains(other.transform))
        //{
        //    var idx = targetList.IndexOf(other.transform);

        //    if (other.transform.Equals(targetList[idx]))
        //    {
        //        targetList[idx].gameObject.SetActive(false);
        //        groundMeshRenderer.material = targetMaterials[idx];
        //        if (idx < gateList.Count)
        //            gateList[idx].gameObject.SetActive(false);

        //        if (idx == targetList.Count - 1)
        //        {
        //            AddReward(+2f);
        //            EndEpisode();
        //        }
        //        else
        //        {
        //            AddReward(+1f);
        //            targetList[idx + 1].gameObject.SetActive(true);
        //        }
        //    }
        //}
    }

    private (float, float) GetRandomGroundPos()
    {
        var extentsX = groundCol.bounds.extents.x;
        var extentsZ = groundCol.bounds.extents.z;
        var randomRangeX = Random.Range(-extentsX, extentsX);
        var randomRangeZ = Random.Range(-extentsZ, extentsZ);

        return (randomRangeX, randomRangeZ);
    }

    private void InitPlayer()
    {

        transform.localPosition = new Vector3(0, transform.localPosition.y, 0);

        //var pos = GetRandomGroundPos();

        //transform.localPosition = new Vector3(pos.Item1, transform.localPosition.y, transform.localPosition.z);
        //transform.localRotation = Quaternion.identity;
    }

    private void InitTrainEnvironment()
    {
        foreach (var target in targetList)
            target.gameObject.SetActive(true);

        foreach (var gate in gateList)
            gate.gameObject.SetActive(true);

        //foreach(var target in targetList)
        //{
        //    var randPos = GetRandomGroundPos();
        //    target.localPosition = new Vector3(randPos.Item1, target.localPosition.y, target.localPosition.z);
        //    target.gameObject.SetActive(false);
        //}
        //targetList[0].gameObject.SetActive(true);
    }

    private void MoveAgent(ActionBuffers actionBuffers)
    {
        var continuousAction = actionBuffers.ContinuousActions;

        var dir = new Vector3(continuousAction[0], 0f, continuousAction[1]).normalized;
        transform.localPosition += dir * Time.deltaTime * 1;

        if (dir.magnitude > 0f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
        }

        string animParam = dir.magnitude >= 0.1f ? "Walk" : "Idle";
        animator.SetTrigger(animParam);

        AddReward(-0.0001f);
    }
}
