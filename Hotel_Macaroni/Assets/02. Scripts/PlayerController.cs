using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("MovementOption")]
    [SerializeField] float walkSpeed = 3f;      // 걷기 속도
    [SerializeField] float runSpeed = 5f;       // 뛰기 속도
    [SerializeField] float mouseSpeed = 8f;     // 마우스 회전 속도
    [SerializeField] float gravity = 10f;       // 중력 값

    [SerializeField] float camHeight = 0.5f;    // 기본 카메라 높이
    [SerializeField] float crouchHeight = 0.2f;    // 숙였을 때 카메라 높이
    [SerializeField] float maxLookAngle = 60f;  // 상하 회전 각도 제한
    [SerializeField] float idleCamShake = 0.5f; // idle상태의 카메라 흔들림 값
    [SerializeField] float walkCamShake = 5f;   // 걷기 상태일 때 카메라 흔들림 값
    [SerializeField] float runCamShake = 10;    // 달리기 상태일 때 카메라 흔들림 값
    

    private CharacterController controller;     // CharacterController 참조
    private Camera mainCamera;                  // 카메라 참조
    private Vector3 moveDirection;              // 실제 이동 방향
    private float mouseX;                       // 마우스 X축 회전 값 (좌우 회전)
    private float mouseY;                       // 마우스 Y축 회전 값 (상하 회전)
    private Vector2 inputVector;                // 이동 입력 (WASD)

    [Header("PlayerState")]
    public int nowFloor = 7;
    [SerializeField] private bool isMove;       // 이동 상태 여부
    [SerializeField] private bool isRunning;    // RUN 상태 여부
    [SerializeField] private bool isCrouch;     //숙이기 상태 여부
    [SerializeField] private bool isDontMove = false;       //이동 불가 상태 여부 (기본 false)
    [SerializeField] private bool isFlashActive = false;    //손전등 사용 상태 여부 (기본 false)

    private ReticleManager reticleManager;      // 조준점 매니저
    private GameManager gameManager;

    //손전등 관련 변수
    [SerializeField] private GameObject flashLight; // 손전등 게임 오브젝트
    [SerializeField] float flashLightLife = 10f; // 손전등 수명
    private float nowFlashLightLife;
    private Coroutine flashLightCoroutine;


    //테스트용 시네머신 카메라
    [SerializeField] private Transform playerCamPos;                                // 플레이어 시네머신 카메라 트래킹 타겟
    [SerializeField] private CinemachineCamera playerCam;
    public CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;
    public float flashLightSpeed = 10f;
    [SerializeField] private GameObject playerAim;
    [SerializeField] private CinemachineCamera focusCam;
    [SerializeField] private bool isFocusCamActive;
    [SerializeField] private GameObject focusObject;

    //테스트용 변수
    public UnityEvent onFlashChange;

    public GameObject noteUI; //일단 플레이어 컨트롤러에서 임시로 사용

    /*private float smoothTime = 0.3f; // 부드럽게 이동할 시간
    private Vector3 velocity = Vector3.zero; // 현재 속도*/


    private void Awake()
    {
        controller = GetComponent<CharacterController>();  // CharacterController 초기화
        reticleManager = GetComponent<ReticleManager>();
        gameManager = GameManager.instance;
        mainCamera = Camera.main;                          // 메인 카메라 참조
        CursorState(false);
    }

    private void Start()
    {
        if (!isFlashActive) flashLight.SetActive(false);
        nowFlashLightLife = flashLightLife;
        isCrouch = false;
    }

    private void FixedUpdate()
    {
        Movement();
        Rotation();
    }

    // Move
    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log("이동중");
        if (!isDontMove)
        {
            inputVector = context.ReadValue<Vector2>();

            isMove = inputVector != Vector2.zero;

            if (!isMove)
            {
                cinemachineBasicMultiChannelPerlin.AmplitudeGain = idleCamShake;
                cinemachineBasicMultiChannelPerlin.FrequencyGain = idleCamShake;
            }
        } 
    }

    // Look
    public void OnLook(InputAction.CallbackContext context)
    {
        if (!isDontMove)
        {
            Vector2 mouseDelta = context.ReadValue<Vector2>();
            mouseX += mouseDelta.x * mouseSpeed * Time.deltaTime;   // X축
            mouseY += mouseDelta.y * mouseSpeed * Time.deltaTime;   // Y축
        }
    }

    // Run
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isRunning = true;
            Debug.Log("달리기 상태 + 시간체크");
            float currentHour = gameManager.GetTime("hour");
            if(isRunning && (currentHour >= 22f || currentHour < 6f))
            {
                Debug.Log("뛰어다니면 안되는 시간대입니다.");
                gameManager.eventManager.RandomDebuffOn();
            }
        }
        else if (context.canceled)
        {
            isRunning = false;
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isCrouch = true;
            Debug.Log("숙이기 상태");
            playerCamPos.localPosition = new Vector3(playerCamPos.localPosition.x, crouchHeight, playerCamPos.localPosition.z);
            //Vector3 targetPosition = new Vector3(playerCamPos.localPosition.x, crouchHeight, playerCamPos.localPosition.z);
            //playerCamPos.localPosition = Vector3.SmoothDamp(playerCamPos.localPosition, targetPosition, ref velocity, smoothTime);
        }
        else if (context.canceled)
        {
            isCrouch= false;
            Debug.Log("숙이기 해제");
            playerCamPos.localPosition = new Vector3(playerCamPos.localPosition.x, camHeight, playerCamPos.localPosition.z);
            //Vector3 targetPosition = new Vector3(playerCamPos.localPosition.x, camHeight, playerCamPos.localPosition.z);
            //playerCamPos.localPosition = Vector3.SmoothDamp(playerCamPos.localPosition, targetPosition, ref velocity, smoothTime);
        }
    }

    public void Movement()
    {
        // 현재 이동 속도 결정 (걷기 또는 뛰기)
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        if (isMove)
        {
            float currentShakeValue = isRunning ? runCamShake : walkCamShake;
            cinemachineBasicMultiChannelPerlin.AmplitudeGain = currentShakeValue;
            cinemachineBasicMultiChannelPerlin.FrequencyGain = currentShakeValue;
        }
        else
        {
            
        }
        

        // 캐릭터가 땅에 있을 때
        if (controller.isGrounded)
        {
            // 입력 벡터를 이동 벡터로 변환
            moveDirection = new Vector3(inputVector.x, 0, inputVector.y);
            moveDirection = transform.TransformDirection(moveDirection) * currentSpeed;
        }
        else
        {
            // 땅과 떨어져 있으면 중력 적용
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // CharacterController를 통해 이동
        if (controller.enabled == true)
        {
            controller.Move(moveDirection * Time.deltaTime);
        }
    }

    public void Rotation()
    {
        // 캐릭터 좌우 회전 처리
        /*transform.localEulerAngles = new Vector3(0, mouseX, 0);

        // 카메라 상하 회전 처리 및 각도 제한
        mouseY = Mathf.Clamp(mouseY, -maxLookAngle, maxLookAngle);
        playerCam.transform.localEulerAngles = new Vector3(-mouseY, 0, 0);*/

        //테스트

        mouseY = Mathf.Clamp(mouseY, -maxLookAngle, maxLookAngle);

        // 플래시라이트 회전 처리 (플래시라이트가 먼저 회전)
        //Vector3 flashlightTargetRotation = new Vector3(-mouseY, mouseX, 0);
        //flashLight.transform.localRotation = Quaternion.Lerp(flashLight.transform.localRotation, Quaternion.Euler(flashlightTargetRotation), flashLightSpeed * Time.deltaTime);

        // 카메라 상하 회전 처리
        playerCam.transform.localEulerAngles = new Vector3(-mouseY, 0, 0);
        //playerAim.transform.localEulerAngles = new Vector3(-mouseY, 0, 0);

        // 캐릭터 좌우 회전 처리
        transform.localEulerAngles = new Vector3(0, mouseX, 0);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            reticleManager.InteractionCheck();
        }
        //Debug.Log("Test");
    }

    public void OnFlashLight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            bool isActive = !flashLight.activeSelf;
            flashLight.SetActive(isActive);
            isFlashActive = isActive;
            onFlashChange.Invoke();
            if(isFlashActive && flashLightCoroutine == null)
            {
                flashLightCoroutine = StartCoroutine(WorkingFlashLight());
            }
            else if (!isFlashActive)
            {
                StopFlashCoroutine();
            }
        }
    }

    public void OnNoteOpen(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //if (GameManager.instance.nowOpenUI != null) return;

            bool isActive = !noteUI.activeSelf;

            noteUI.SetActive(isActive);
            if (isActive)
            {
                GameManager.instance.nowOpenUI = noteUI;
            }
            else
            {
                GameManager.instance.nowOpenUI = null;
            }

            PlayerDontMove(isActive);
            CursorState(isActive);
        }
    }

    public void CancelButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("취소버튼");
            int _nowUIStack = gameManager.uiManager.NowUIStackCheck();
            if(_nowUIStack > 0)
            {
                gameManager.uiManager.CheckUiClose();

            }
            else if(_nowUIStack == 0 && isFocusCamActive)
            {
                focusCam.Priority = -1;
                //focusCam.Target.TrackingTarget = null;
                isFocusCamActive = false;
                
            }

            int uiCheck = gameManager.uiManager.NowUIStackCheck();
            if(uiCheck == 0 && !isFocusCamActive)
            {
                CursorState(false);
                PlayerDontMove(false);
            }
            /*if (isFocusCamActive)
            {
                focusCam.Priority = -1;
                //focusCam.Target.TrackingTarget = null;
                isFocusCamActive = false;
                CursorState(false);
                PlayerDontMove(false);
            }
            else
            {

            }*/
        }
    }

    public void PlayerDontMove(bool state)
    {
        if (state) //움직임 제한
        {
            isDontMove = true;
            inputVector = Vector3.zero;
        }
        else //움직임 제한 해제
        {
            isDontMove = false;
        }
    }

    public void CursorState(bool state)
    {
        Cursor.visible = state;
        GameManager.instance.playerController.isDontMove = state;
        if (state) //마우스 커서 활성화
        {
            Cursor.lockState = CursorLockMode.None;

        }
        else //마우스 커서 비활성화
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public bool FlashLightCheck()
    {
        bool state = isFlashActive;
        return (state);
    }

    public void FocusTarget(Transform transform, GameObject targetObject)
    {
        if (focusCam == null) return;
        isFocusCamActive = true;
        focusCam.gameObject.transform.position = transform.position;
        focusCam.Target.TrackingTarget = targetObject.transform;
        focusCam.Priority = 2;
        CursorState(true);
        PlayerDontMove(true);
    }

    private IEnumerator WorkingFlashLight()
    {
        while (nowFlashLightLife > 0)
        {
            nowFlashLightLife -= Time.deltaTime;
            yield return null;
        }

        flashLight.SetActive(false);
        isFlashActive = false;
        onFlashChange.Invoke();
        flashLightCoroutine = null;
    }

    private void StopFlashCoroutine()
    {
        if(flashLightCoroutine != null)
        {
            StopCoroutine(flashLightCoroutine);
            flashLightCoroutine = null;
        }
    }

    public void ResetFlashLight()
    {
        nowFlashLightLife = flashLightLife;
    }

    public void PlayerMoveSpeed(float _walkSpeed, float _runSpeed)
    {
        walkSpeed = _walkSpeed;
        runSpeed = _runSpeed;
    }
}