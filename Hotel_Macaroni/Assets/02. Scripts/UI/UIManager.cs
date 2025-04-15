using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum UIType
{
    Note,
    Inventory,
    Safe,
    Interphone
}

public class UIManager : Singleton<UIManager>
{
    [System.Serializable]
    public struct UIPanelInfo
    {
        public UIType type;
        public GameObject uiObject;
    }

    public List<UIPanelInfo> uiPanelInfos = new List<UIPanelInfo>();

    public GameObject mainNoteUI;
    public GameObject[] noteUIObjects;
    public Toggle[] noteUIToggles;
    public GameObject reticleUI;
    public GameObject imageInteractUI;
    public InspectSystem inspectSystem;

    public GameObject interphoneUI;

    private Stack<GameObject> uiStack = new Stack<GameObject>();
    
    private PlayerController playerController;

    //private Dictionary<Type, UnityEngine.Object[]> uiObjects = new Dictionary<Type, UnityEngine.Object[]>();
    
    private Dictionary<UIType, GameObject> uiDictionary = new Dictionary<UIType, GameObject>();
    
    //인벤토리 버튼 할당
    //토글에 ui를 연결해야함

    private void Awake()
    {
        InitializeUIDictionary();
    }

    private void Start()
    {
        CloseAll();
        BindingToggleUI();
        playerController = GameManager.instance.playerController;

        /*if(inspectSystem == null)
        {
            inspectSystem = FindObjectsOfType<>
        }*/
    }

    private void InitializeUIDictionary()
    {
        uiDictionary.Clear();
        foreach(var info in uiPanelInfos)
        {
            if(info.uiObject != null && !uiDictionary.ContainsKey(info.type))
            {
                uiDictionary.Add(info.type, info.uiObject);
                info.uiObject.SetActive(false); //초기값으로 비활성화 처리
            }
            else
            {
                Debug.LogError($"UIManager : UI Type[{info.type}] 가 이미 있거나, ui오브젝트가 비어있음.");
            }
        }
    }

    /// <summary>
    /// 해당 메소드는 ui매니저를 통해 딕셔너리에 있는 ui를 열기 위함이고,
    /// 다른 스크립트에서 바로 열거나(버튼 누르면 여는식), 아직 교체하기 전 방식은
    /// 스크립트를 열고, AddOpenUI를 통해 전달해야 함.
    /// </summary>
    public void OpenUI(UIType _type)
    {
        if(uiDictionary.TryGetValue(_type, out GameObject _uiObject))
        {
            _uiObject.SetActive(true);
            AddOpenUI(_uiObject, true);
        }
        else
        {
            Debug.LogError($"UIManager : UI Type[{_type}]을 딕셔너리에서 찾지 못했거나, ui오브젝트가 비어있음.");
        }
    }

    public void OpenNote(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            CloseAll(); //리셋

            //메인 노트 ui의 상태확인(반대) => 버튼을 눌럿을 때, 꺼져있을경우 true를 반환(켜기)
            bool isActive = !mainNoteUI.activeSelf; 
            mainNoteUI.SetActive(isActive);
            playerController.PlayerDontMove(isActive);
            playerController.CursorState(isActive);

            if (isActive) //노트ui 켜기
            {
                noteUIObjects[0].SetActive(true);
                noteUIToggles[0].isOn = true;
                
                AddOpenUI(mainNoteUI, true);
            }
            else //노트 ui 끄기
            {
                if (inspectSystem.isInspectOn) inspectSystem.DisableInspectView();  
                AddOpenUI(mainNoteUI, false);
                CheckStackDeactivate();
            }

            
        }
    }

    private void OnToggleChanged(bool _isOn, int _Index)
    {
        if (_isOn)
        {
            foreach(GameObject ui in noteUIObjects)
            {
                ui.SetActive(false);
            }
            noteUIObjects[_Index].SetActive(true);
        }
    }

    private void CloseAll()
    {
        for (int i = 0; i < noteUIObjects.Length; i++)
        {
            noteUIObjects[i].SetActive(false);
            noteUIToggles[i].isOn = false;
        }
    }

    private void BindingToggleUI()
    {
        for(int i = 0;i < noteUIToggles.Length;i++)
        {
            int index = i;
            noteUIToggles[index].onValueChanged.AddListener((isOn) => OnToggleChanged(isOn, index));
        }
    }

    public void AddOpenUI(GameObject openUI, bool add) //false는 제거
    {
        if (add)
        {
            if (!uiStack.Contains(openUI))
            {
                uiStack.Push(openUI);
                Debug.Log(openUI + "켜짐");
            }
        }
        else
        {
            if(uiStack.Count > 0 && uiStack.Contains(openUI))
            {
                GameObject lastOBJ =  uiStack.Peek();
                //마지막에 들어간 ui스택이 열려있는 ui라면
                if(lastOBJ == openUI)
                {
                    //제거
                    uiStack.Pop();
                }
                CheckStackDeactivate();
            }
        }
    }

    public void SubtractOpenUI(GameObject openUI)
    {
        //if()
    }

    public int NowUIStackCheck()
    {
        int nowUIStack = uiStack.Count;
        return nowUIStack;
    }

    public void CheckUiClose()
    {
        if (uiStack.Count > 0)
        {
            GameObject LastOpenUI = uiStack.Pop();
            LastOpenUI.SetActive(false);
        }

        CheckStackDeactivate();
    }

    private void CheckStackDeactivate()
    {
        if (uiStack.Count == 0) return;
        Stack<GameObject> tempStack = new Stack<GameObject>();

        while (uiStack.Count > 0)
        {
            GameObject currentObject = uiStack.Pop();
            if (currentObject.activeSelf)
            {
                tempStack.Push(currentObject);
            }
            else
            {
                Debug.Log(currentObject.name + "해당 ui오브젝트는 비활성화여서 제거");
            }
        }

        while (tempStack.Count > 0)
        {
            uiStack.Push(tempStack.Pop());
        }
    }
}
