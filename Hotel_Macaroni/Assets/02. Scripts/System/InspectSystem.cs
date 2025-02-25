using NUnit.Framework.Internal;
using UnityEngine;

public class InspectSystem : MonoBehaviour
{
    public Transform SpawnPos;
    public float rotationSpeed = 30f;
    private Vector3 originRot;
    private Vector3 prevMousePos;
    [SerializeField] private GameObject nowInspectItem;
    [SerializeField] private InspectObject inspectObject;
    public CanvasGroup mainCanvasGroup;
    public string inspectLayer = "RenderTexture";
    [SerializeField] private InspectObject InspectObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originRot = Vector3.zero;

    }

    private void OnEnable()
    {
        GameManager.instance.uiManager.AddOpenUI(this.transform.gameObject, true);
    }

    private void OnDisable()
    {
        if(nowInspectItem != null)
        {
            Destroy(nowInspectItem.gameObject);
            mainCanvasGroup.alpha = 1;

        }
    }


    public void EnableInspectView(GameObject _inspectTarget)
    {
        GameObject inspectTarget = Instantiate(_inspectTarget, SpawnPos.position, Quaternion.Euler(0,0,0));
        inspectTarget.transform.parent = this.transform;
        int newLayer = LayerMask.NameToLayer(inspectLayer);
        inspectTarget.layer = newLayer;
        inspectTarget.AddComponent<InspectObject>();
        nowInspectItem = inspectTarget;
    }
}
