using UnityEngine;
using RootMotion.FinalIK;

public class InteractionControl : MonoBehaviour
{
    public InteractionSystem interactionSystem;

    private void OnGUI()
    {
        int closestTriggerIndex = interactionSystem.GetClosestTriggerIndex();

        if (closestTriggerIndex == -1) return;

        GUILayout.Label("Press E to start interaction");

        if (Input.GetKeyDown(KeyCode.E))
        {
            interactionSystem.TriggerInteraction(closestTriggerIndex, false);
        }
    }
}
