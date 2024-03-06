using System.Collections;
using RootMotion.FinalIK;
using UnityEngine;

public class Cat : MonoBehaviour
{
    public InteractionObject ball;
    public OffsetPose holdPose; 

    private float holdWeight;
    private FullBodyBipedIK ik;
    private InteractionSystem interactionSystem;

    IEnumerator OnInteract()
    {
        ik = ball.lastUsedInteractionSystem.GetComponent<FullBodyBipedIK>();
        interactionSystem = GetComponent<InteractionSystem>();

        // set right shoulder, hand
        ik.solver.rightHandEffector.positionWeight = 0.5f;
        ik.solver.rightShoulderEffector.positionWeight = 0.03f;

        while (holdWeight < 1f)
        {
            holdWeight += Time.deltaTime;
            yield return null;
        }
    }

    private void LateUpdate()
    {
        if (ik == null) return;

        // apply OffsetPose values
        holdPose.Apply(ik.solver, holdWeight, ik.transform.rotation);
        //ik.solver.leftHandEffector.positionOffset += ik.transform.rotation * holdOffset * holdWeight;
    }

    private void Update()
    {
        if (ik == null) return;

        if (Input.GetKeyDown(KeyCode.D)) interactionSystem.ResumeAll();
    }

    IEnumerator Drop()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        transform.parent = null;

        while (holdWeight > 0f)
        {
            holdWeight -= Time.deltaTime * 3f;
            yield return null;
        }
    }
}
