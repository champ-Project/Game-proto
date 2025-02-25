using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject mainNoteUI;
    public GameObject[] noteUIObjects;
    public Toggle[] noteUIToggles;
    public GameObject reticleUI;
    public GameObject imageInteractUI;
    public GameObject inspectSystem;

    private Stack<GameObject> uiStack = new Stack<GameObject>();
    
    private PlayerController playerController;

    //private Dictionary<Type, UnityEngine.Object[]> uiObjects = new Dictionary<Type, UnityEngine.Object[]>();

    //인벤토리 버튼 할당
    //토글에 ui를 연결해야함

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
            else
            {
                if(inspectSystem.activeSelf) inspectSystem.SetActive(false);    
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
            if (!uiStack.Contains(openUI))
            {
                if (uiStack.Count == 1)
                {
                    GameObject lastOBJ =  uiStack.Pop();
                }
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
