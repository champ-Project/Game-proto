using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("MovementOption")]
    [SerializeField] float walkSpeed = 5f;      // 걷기 속도
    [SerializeField] float runSpeed = 10f;      // 뛰기 속도
    [SerializeField] float mouseSpeed = 8f;     // 마우스 회전 속도
    [SerializeField] float gravity = 10f;       // 중력 값
    [SerializeField] float maxLookAngle = 60f;  // 상하 회전 각도 제한
    [SerializeField] float idleCamShake = 0.5f;
    [SerializeField] float walkCamShake = 5f;
    [SerializeField] float runCamShake = 10;

    private CharacterController controller;     // CharacterController 참조
    private Camera mainCamera;                  // 카메라 참조
    private Vector3 moveDirection;              // 실제 이동 방향
    private float mouseX;                       // 마우스 X축 회전 값 (좌우 회전)
    private float mouseY;                       // 마우스 Y축 회전 값 (상하 회전)
    private Vector2 inputVector;                // 이동 입력 (WASD)

    [Header("PlayerState")]
    [SerializeField] private bool isMove;       //이동 상태 여부
    [SerializeField] private bool isRunning;    // RUN 상태 여부
    [SerializeField] private bool isDontMove = false;       //이동 불가 상태 여부 (기본 false)
    [SerializeField] private bool isFlashActive = false;    //손전등 사용 상태 여부 (기본 false)

    private ReticleManager reticleManager;
    [SerializeField] private GameObject flashLight;
    

    //테스트용 시네머신 카메라
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

    

    private void Awake()
    {
        controller = GetComponent<CharacterController>();  // CharacterController 초기화
        reticleManager = GetComponent<ReticleManager>();
        mainCamera = Camera.main;                          // 메인 카메라 참조
        CursorState(false);
    }

    private void Start()
    {
        if (!isFlashActive) flashLight.SetActive(false);
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
        }
        else if (context.canceled)
        {
            isRunning = false;
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
        Debug.Log("Test");
    }

    public void OnFlashLight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            bool isActive = !flashLight.activeSelf;
            flashLight.SetActive(isActive);
            isFlashActive = isActive;
            onFlashChange.Invoke();
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
            if (isFocusCamActive)
            {
                focusCam.Priority = -1;
                //focusCam.Target.TrackingTarget = null;
                isFocusCamActive = false;
                CursorState(false);
                PlayerDontMove(false);
            }
            else
            {

            }
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
}