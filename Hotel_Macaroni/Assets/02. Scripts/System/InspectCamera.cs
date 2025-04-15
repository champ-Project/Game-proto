using UnityEngine;

public class InspectCamera : MonoBehaviour
{
    public InspectSystem inspectSystem;

    private void OnEnable()
    {
        GameManager.instance.uiManager.AddOpenUI(this.transform.gameObject, true);
    }

    private void OnDisable()
    {
        if(inspectSystem != null)
        {
            inspectSystem.DisableInspectView();
        }
    }
}
