using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.UI;

public class ManageIK : MonoBehaviour
{   
    private float lookWeight;
    private LookAtIK look;

    public GameObject headPivot;
    public Transform target;

    public float pivotRotY;
    public float distance;
    public Button touchCatBtn;

    private void Start()
    {
        look = GetComponent<LookAtIK>();
    }

    private void Update()
    {
        headPivot.transform.LookAt(target);
        
        pivotRotY = headPivot.transform.localRotation.y;
        distance = Vector3.Distance(headPivot.transform.position, target.position);
        if (pivotRotY < 0.5f && pivotRotY > -0.5f && distance < 2f)
        {
            // if (!touchCatBtn.gameObject.activeSelf)
            //     touchCatBtn.gameObject.SetActive(true);
            look.solver.IKPosition = target.position;
            look.solver.IKPositionWeight = Mathf.Lerp(lookWeight, 1, Time.deltaTime * 2.5f);
            lookWeight = look.solver.IKPositionWeight;
        }
        else
        {
            // if (touchCatBtn.gameObject.activeSelf)
            //     touchCatBtn.gameObject.SetActive(false);
            look.solver.IKPositionWeight = 0f;
        }
            
    }
}
