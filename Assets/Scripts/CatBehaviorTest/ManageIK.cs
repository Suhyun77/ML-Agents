using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using RootMotion.FinalIK;
using UnityEngine;

public class ManageIK : MonoBehaviour
{   
    private float lookWeight;
    private LookAtIK look;

    public GameObject headPivot;
    public Transform target;

    private void Start()
    {
        look = GetComponent<LookAtIK>();
    }

    private void Update()
    {
        headPivot.transform.LookAt(target);
        
        float pivotRotY = headPivot.transform.localRotation.y;
        float distance = Vector3.Distance(headPivot.transform.position, target.position);
        if (pivotRotY < 0.5f && pivotRotY > -0.5f && distance < 2f)
        {
            look.solver.IKPosition = target.position;
            look.solver.IKPositionWeight = Mathf.Lerp(lookWeight, 1, Time.deltaTime * 2.5f);
            lookWeight = look.solver.IKPositionWeight;
        }
        else
            look.solver.IKPositionWeight = 0f;
    }
}
