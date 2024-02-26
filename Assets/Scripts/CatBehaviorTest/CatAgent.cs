using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine.UI;

/*
 * 축구 Agent 블로그 : https://ojui.tistory.com/14
 */

public class CatAgent : Agent
{
    #region Members
    [SerializeField] Collider groundCol;
    [SerializeField] MeshRenderer groundMeshRenderer;
    [SerializeField] Transform targetRoot;
    [SerializeField] List<Transform> targetList;
    [SerializeField] List<Material> targetMaterials;

    [SerializeField] Button feedBtn;
    [SerializeField] GameObject feedObj;

    private Animator animator;
    #endregion

    #region Agent Method
    public override void Initialize()
    {
        animator = GetComponent<Animator>();
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

        feedBtn.onClick.AddListener(() => feedObj.SetActive(true));
    }

    public override void OnEpisodeBegin()
    {
        //transform.localPosition = new Vector3(0, transform.localPosition.y, 0);
        InitPlayer();
        InitTargets();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);

        foreach(var target in targetList)
            sensor.AddObservation(target.transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)  // Continuous
    {
        #region continuous ver
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        var dir = new Vector3(moveX, 0f, moveZ).normalized;
        transform.localPosition += dir * Time.deltaTime * 1;

        if (dir.magnitude > 0f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
        }
        
        string animParam = dir.magnitude >= 0.1f ? "Walk" : "Idle";
        animator.SetTrigger(animParam);
        #endregion
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
        //if (other.CompareTag("Wall"))
        //{
        //    SetReward(-1f);
        //    EndEpisode();
        //}

        if (targetList.Contains(other.transform))
        {
            var idx = targetList.IndexOf(other.transform);

            if (other.transform.Equals(targetList[idx]))
            {
                //var reward = idx == 0 ? 1 : 3;
                SetReward(idx+1);
                groundMeshRenderer.material = targetMaterials[idx];
                //other.transform.gameObject.SetActive(false);

                if (idx+1 == 3)
                    EndEpisode();
            }
        }
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
        var pos = GetRandomGroundPos();

        transform.localPosition = new Vector3(pos.Item1, transform.localPosition.y, transform.localPosition.z);
        transform.localRotation = Quaternion.identity;
    }

    private void InitTargets()
    {
        foreach(var target in targetList)
        {
            var randPos = GetRandomGroundPos();
            target.localPosition = new Vector3(randPos.Item1, target.localPosition.y, target.localPosition.z);
        }
    }
}
