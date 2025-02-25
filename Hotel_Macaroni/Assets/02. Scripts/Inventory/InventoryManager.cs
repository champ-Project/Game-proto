using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public CanvasGroup mainCanvasGroup;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject flashLight;
    [SerializeField] private bool isFlashLightHas;
    [SerializeField] private List<GameObject> inventoryItems = new List<GameObject>();
    [SerializeField] private List<ItemData> invenItemDatas = new List<ItemData>();
    [SerializeField] private List<InventorySlot> inventorySlots = new List<InventorySlot>();
    [SerializeField] private Sprite unknownItemImage;
    [SerializeField] private Image flashLightIcon;
    private PlayerController playerController;
    private Color changeColor;

    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private Toggle currentSelectToggle;
    [SerializeField] private ItemData selectItemData;

    [Header("아이템 디테일")]
    [SerializeField] private Image viewItemImage;
    [SerializeField] private Text viewItemNameText;
    [SerializeField] private Text viewItemInfoText;

    [SerializeField] private GameObject inspectCamera;
    [SerializeField] private InspectSystem inspectSystem;
    [SerializeField] private Button viewDetailBtn;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        inspectSystem = inspectCamera.GetComponentInChildren<InspectSystem>();
    }

    private void Start()
    {
        
        SetInventorySlotToggles();
        viewDetailBtn.onClick.AddListener(InspectViewActive);
        //if(toggleGroup == null) toggleGroup = gameObject.GetComponent<ToggleGroup>();
        //playerController = GameManager.instance.playerController;
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //if (GameManager.instance.nowOpenUI != null) return;

            
            bool isActive = !inventoryUI.activeSelf;

            if (isActive) 
            {
                GameManager.instance.nowOpenUI = inventoryUI;
                GameManager.instance.uiManager.AddOpenUI(inventoryUI, true);
            }
            else 
            {
                GameManager.instance.nowOpenUI = null;
            }

            inventoryUI.SetActive(isActive);
            playerController.PlayerDontMove(isActive);
            playerController.CursorState(isActive);

            if(currentSelectToggle != null)
            {
                currentSelectToggle.isOn = !isActive;
                currentSelectToggle = null;
            }
            /*Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            GameManager.instance.playerController.isDontMove = true;*/
        }
    }

    public void GetItem(GameObject _gameObject)
    {
        //inventoryItems.Add(_gameObject);
        var _itemdata = _gameObject.GetComponent<ItemDataSet>().thisItemData;
        if (_itemdata == null)
        {
            Debug.LogError("해당 오브젝트에 ItemData없음");
            return;
        }

        if (_itemdata.itemName == "FlashLight")
        {
            if(flashLightIcon != null && flashLightIcon.gameObject.activeSelf == false)
            {
                flashLightIcon.gameObject.SetActive(true);
            }
            playerController.ResetFlashLight();
            Destroy(_gameObject);
            return;
        }
        invenItemDatas.Add(_itemdata);
        AddItemToInventory(_itemdata);
        Debug.Log("아이템확인" + _gameObject.name);
        Destroy(_gameObject);
        Debug.Log("필드 아이템파괴");

        Debug.Log(invenItemDatas[0].itemName + invenItemDatas[0].itemCode);
    }

    public void AddItemToInventory(ItemData _itemData)
    {
        for(int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].ItemData == null)
            {
                changeColor = inventorySlots[i].itemImage.color;
                changeColor.a = 1;
                inventorySlots[i].ItemData = _itemData;
                if (_itemData.itemImage != null)
                {
                    inventorySlots[i].itemImage.sprite = _itemData.itemImage;
                }
                else
                {
                    inventorySlots[i].itemImage.sprite = unknownItemImage;
                }

                inventorySlots[i].itemImage.color = changeColor;
                Debug.Log("아이템 추가" + _itemData.itemName);
                break;
            }
        }
    }

    public bool CheckInventory(string _itemName)
    {
        bool isHave = false;
        foreach(var data in inventorySlots)
        {
            if(data.ItemData != null)
            {
                if (data.ItemData.itemName == _itemName)
                {
                    Debug.Log(_itemName + "아이템 존재함");
                    isHave = true;
                    break;
                }
            }
        }
        if (!isHave)
        {
            Debug.Log(_itemName + "아이템 없음");
        }
        return isHave;
    }

    public void RemoveItem(string _itemName)
    {
        for(int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].ItemData != null && inventorySlots[i].ItemData.itemName == _itemName)
            {
                Debug.Log("확인1");
                inventorySlots[i].ItemData = null;
                inventorySlots[i].itemImage.sprite = null;
                ShiftItems(i);
                return;
            }
        }
        Debug.Log(_itemName + "아이템 제거됨");
    }

    public void ShiftItems(int startIndex)
    {
        Debug.Log("확인2");
        for (int i = startIndex; i < inventorySlots.Count - 1; i++)
        {
            /*if(inventorySlots[i+1].ItemData == null)
            {
                Debug.Log("확인2-1");
                break;
            }*/

            if (inventorySlots[i+1].ItemData != null) //다음 인벤토리 슬롯이 빈칸이 아닐때
            {
                inventorySlots[i].ItemData = inventorySlots[i + 1].ItemData; //현재 칸의 데이터를 다음 칸에서 끌어옴
                inventorySlots[i].itemImage.sprite = inventorySlots[i + 1].itemImage.sprite; //이미지 교체
                inventorySlots[i + 1].ItemData = null;
                inventorySlots[i + 1].itemImage.sprite = null;
            }
            else
            {
                Debug.Log("확인3");
                changeColor = inventorySlots[i].itemImage.color;
                changeColor.a = 0;
                inventorySlots[i].itemImage.color = changeColor;
            }           
        }
    }

    private void CheckLightIcon()
    {

    }

    private void SetInventorySlotToggles()
    {
        foreach(var slot in inventorySlots)
        {
            Toggle toggle = slot.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener((isOn) => ToggleChanged(toggle, isOn));
        }
    }

    private void ToggleChanged(Toggle toggle, bool isOn)
    {
        InventorySlot inventorySlot = toggle.GetComponent<InventorySlot>();
        if (toggle.isOn)
        {
            currentSelectToggle = toggle;
            //Debug.Log(toggle.name + "해당 토글체크");
            
            if (inventorySlots.Contains(inventorySlot) == true && inventorySlot.ItemData != null)
            {
                if (inventorySlot.ItemData.name == null || inventorySlot.ItemData.iteminfo == null || inventorySlot.ItemData.itemImage == null)
                {
                    Debug.LogError("아이템 데이터에 공백 존재");
                    return;
                }
                viewItemImage.enabled = true;
                selectItemData = inventorySlot.ItemData;
                viewItemNameText.text = selectItemData.itemName;
                viewItemInfoText.text = selectItemData.iteminfo;
                viewItemImage.sprite = selectItemData.itemImage;
            }
            else
            {
                Debug.Log("슬롯에 아이템이 없음");
            }
        }
        else
        {
            
            if (selectItemData !=null && selectItemData == inventorySlot.ItemData)
            {
                currentSelectToggle = null;
                viewItemImage.enabled=false;
                selectItemData = null;
                viewItemNameText.text = "아이템 이름";
                viewItemInfoText.text = "아이템 설명";
                viewItemImage.sprite = null;
            }
        }
    }

    private void InspectViewActive()
    {
        if (selectItemData == null) 
        {
            Debug.Log("선택된 아이템 없음");
            return;
        }

        if (inspectSystem != null && selectItemData.prefab != null)
        {
            mainCanvasGroup.alpha = 0;
            inspectSystem.gameObject.SetActive(true);
            inspectSystem.EnableInspectView(selectItemData.prefab);
        }    
    }
}
