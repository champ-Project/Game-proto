using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject player;

    public PlayerController playerController;
    public ReticleManager reticleManager;
    public InventoryManager inventoryManager;
    public SafeManager safeManager;
    public MsgManager msgManager;
    public EventManager eventManager;
    public UIManager uiManager;
    public GuideNoteManager guideNoteManager;

    public bool isGetNote = false; //플레이어가 노트를 획득했는지 확인하는 bool 값
    public string nowPlayerName;

    [SerializeField] private float realTimeToGameTimeRatio = 30f; // 실제 30초가 게임 내 1시간에 해당
    private float gameTime; // 게임 내 시간 (시간 단위)
    private float gameHours; // 게임 내 시간 (시간)
    private float gameMinutes; // 게임 내 시간 (분)

    public int startHours = 12; //초기 시간
    public int startMinutes = 0; //초기 분

    public Text gameTimeText;

    public GameObject nowOpenUI = null;

    public Transform startPos;
    public GameObject deadUI;
    public Text deadReasonText;

    public Animator mainRoomDoor;

    private Stack<GameObject> uiStack = new Stack<GameObject>();

    //public GameObject moveObj;

    private void Awake()
    {
        instance = this;

        playerController = player.GetComponent<PlayerController>();
        reticleManager = player.GetComponent<ReticleManager>();
        inventoryManager = player.GetComponent<InventoryManager>(); 
        safeManager = player.GetComponent<SafeManager>();
        msgManager = GetComponent<MsgManager>();
        eventManager = GetComponent<EventManager>();
    }

    private void Start()
    {
        SetInitialTime(startHours, startMinutes);
        //ChangePlayer();
    }

    private void FixedUpdate()
    {
        GameTimeSystem();
    }

    public void ChangePlayer() //캐릭터 리스폰
    {
        Debug.Log("위치이동");
        //
        //Rigidbody rb = player.GetComponent<Rigidbody>();
        //rb.MovePosition(startPos.transform.position);
        CharacterController characterController = player.GetComponent<CharacterController>();
        characterController.enabled = false;
        player.transform.position = startPos.position;
    }



    private void GameTimeSystem() //게임 시간 시스템
    {
        gameTime += Time.fixedDeltaTime / realTimeToGameTimeRatio; // 고정된 시간 간격으로 업데이트

        // 24시간을 초과하지 않도록 조정
        if (gameTime >= 24f)
        {
            gameTime = 0f; // 하루가 지나면 리셋
        }

        // 시간과 분으로 변환
        gameHours = Mathf.Floor(gameTime); // 정수 부분 (시간)
        gameMinutes = Mathf.Floor((gameTime - gameHours) * 60); // 분 계산

        UpdateTimeText();

        // 게임 내 시간 출력 (디버그용)
        //Debug.Log($"게임 내 시간: {gameHours:00}:{gameMinutes:00}");
    }

    private void SetInitialTime(int hours, int minutes)
    {
        if (hours < 0 || hours >= 24 || minutes < 0 || minutes >= 60)
        {
            Debug.LogWarning("잘못된 시간 입력입니다. 시간은 0~23, 분은 0~59여야 합니다.");
            return;
        }

        // 초기 시간을 게임 시간으로 변환
        gameTime = hours + (minutes / 60f); // 시간과 분을 기반으로 게임 내 시간 설정
    }

    private void UpdateTimeText()
    {
        if (gameTimeText != null)
        {
            gameTimeText.text = $"게임 내 시간: {gameHours:00}:{gameMinutes:00}";
        }
    }

    public float GetTime(string _kind)
    {
        if(_kind == "hour")
        {
            return gameHours;
        }
        else if (_kind == "minute")
        {
            return gameMinutes;
        }
        return gameTime;
    }

    /*private void SpecificTimeCheck() 
    {

    }*/


    public void PlayerDead(string reason)
    {
        if ((mainRoomDoor.GetBool("DoorOpenNeg")) == true || (mainRoomDoor.GetBool("DoorOpenPos")) == true)
        {
            mainRoomDoor.SetBool("DoorOpenNeg", false);
            mainRoomDoor.SetBool("DoorOpenPos", false);
        }

        nowPlayerName = null;
        isGetNote = false;
        
        deadReasonText.text = reason;
        StartCoroutine(PlayerDeadSequence());
    }

    /*private IEnumerator PlayerStartSequence()
    {

    }*/


    private IEnumerator PlayerDeadSequence()
    {
        playerController.PlayerDontMove(true); //이동 불가
        playerController.CursorState(true); // 커서 멈추기
        deadUI.SetActive(true); //사망 UI 켜기
        ChangePlayer(); //플레이어 변경
        yield return new WaitForSeconds(5f);
        deadUI.SetActive(false);
        CharacterController characterController = player.GetComponent<CharacterController>();
        characterController.enabled = true;
        playerController.PlayerDontMove(false);
        playerController.CursorState(false);
        eventManager.filmGrain.intensity.value = 0;
    }

    private void HintPageDrop()
    {

    }

    /*public void AddOpenUI(GameObject openUI)
    {
        if (!uiStack.Contains(openUI))
        {
            uiStack.Push(openUI);
            Debug.Log(openUI + "켜짐");
        }
    }


    //UI매니저로 이동할 가능성이 있음
    public void CheckUiClose()
    {
        if(uiStack.Count > 0)
        {
            GameObject LastOpenUI = uiStack.Pop();
            LastOpenUI.SetActive(false);
        }
    }

    public int NowUIStackCheck()
    {
        int nowUIStack = uiStack.Count;
        return nowUIStack;
    }*/


}
