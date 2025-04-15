using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InspectSystem : MonoBehaviour
{
    [SerializeField] private GameObject inspectCam;
    public bool isInspectOn = false;

    public Transform SpawnPos;
    public float rotationSpeed = 30f;
    private Vector3 originRot;
    //private Vector3 prevMousePos;
    [SerializeField] private GameObject nowInspectItem;
    public CanvasGroup mainCanvasGroup;
    public string inspectLayer = "Inspect";

    public GameObject readIcon;
    public GameObject readUI;
    
    public Text inspectItemText;    //

    /*private void OnEnable()
    {
        GameManager.instance.uiManager.AddOpenUI(this.transform.gameObject, true);
    }*/

    //비활성화시에 인스펙트 아이템 제거
    /*private void OnDisable()
    {
        if(nowInspectItem != null)
        {
            Destroy(nowInspectItem.gameObject);
            mainCanvasGroup.alpha = 1;

        }
    }*/

    public void OnRead(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //인스펙트 사용중, read가능한 경우에
            if(isInspectOn && readIcon.activeSelf == true)
            {
                bool isActive = !readUI.activeSelf;
                readUI.SetActive(isActive);

                if (isActive)
                {
                    GameManager.instance.uiManager.AddOpenUI(readUI, true);
                }
                else
                {
                    GameManager.instance.uiManager.AddOpenUI(readUI, false);
                }
            }
        }
    }


    public void EnableInspectView(ItemData _inspectItemData)
    {
        if (_inspectItemData.prefab == null) return;

        bool isReadable = CheckReadableItem(_inspectItemData);
        readIcon.SetActive(isReadable);

        mainCanvasGroup.alpha = 0; //메인 캔버스 투명화
        inspectCam.gameObject.SetActive(true); //인스펙트 시스템 켜기

        //인스펙트 오브젝트 소환
        GameObject inspectTarget = Instantiate(_inspectItemData.prefab, SpawnPos.position, Quaternion.Euler(0,0,0));
        //해당 스크립트가 부착된 오브젝트의 자식으로
        inspectTarget.transform.parent = this.transform;

        //소환한 오브젝트의 레이어는 Inspect레이어
        int newLayer = LayerMask.NameToLayer(inspectLayer);
        inspectTarget.layer = newLayer;

        //오브젝트 회전에 사용할 InspectObject 컴포넌트 장착
        inspectTarget.AddComponent<InspectObject>();

        //현재 인스펙트중인 아이템으로 임시 저장
        nowInspectItem = inspectTarget;

        GameManager.instance.playerController.PlayerDontMove(true);
        GameManager.instance.playerController.CursorState(true);
        isInspectOn = true;
    }

    public void DisableInspectView()
    {
        if (nowInspectItem != null)
        {
            Destroy(nowInspectItem.gameObject);
            mainCanvasGroup.alpha = 1;
            inspectCam.gameObject.SetActive(false);
            readUI.SetActive(false);
            readIcon.SetActive(false);
            isInspectOn = false;
        }
    }

    private bool CheckReadableItem(ItemData _itemData)
    {
        bool isReadable = false;

        if(_itemData.itemType == ItemType.notePage || _itemData.itemType == ItemType.storyPaper)
        {
            if (_itemData.itemType == ItemType.notePage)
            {
                _itemData.text = GameManager.instance.eventManager.FindEventCSVPart(_itemData.itemCode, "hint1");
            }
            else if (_itemData.itemType == ItemType.storyPaper)
            {
                _itemData.text = GameManager.instance.eventManager.FindStoryCSVPart(_itemData.itemCode, "story");
            }

            //받아온 텍스트가 없을 경우
            if (_itemData.text == null && _itemData.text == "")
            {
                isReadable = false;
            } //있을경우
            else
            {
                isReadable =  true;
                inspectItemText.text = _itemData.text;
            }
        }
        else
        {
            //추후 혹시 노트페이지, 스토리 종이가 아니어도 필요한경우 아래 주석 해제하고 사용
            //텍스트가 null이 아니거나, 혹은 공백이 아닐경우(텍스트가 있는 아이템인 경우)
            /*if(_itemData.text != null || _itemData.text != "")
            {
                isReadable = true;
            }*/
            isReadable = false;
        }

        return isReadable;
    }

    ///
    /// 생각을 해보자... 결국 인스펙트 시스템을 불러서 사용을 하는게 맞다고 보니까
    /// 이 스크립트를 장착한 오브젝트는 활성화 되어있어야 하고..
    ///
}
