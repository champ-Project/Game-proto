using Unity.Cinemachine;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

//플레이어 조준점 스크립트
//Ray, 타겟 오브젝트 저장, 오브젝트 확인 등
public class ReticleManager : MonoBehaviour
{
    public float rayDistance = 3f;          //Ray거리
    private GameObject currentItem;         //현재 조준점에 잡힌 아이템
    private bool isReticalOnItem = false;   //아이템 포착 여부 bool
    public GameObject reticleText;          //조준점 텍스트
    public GameObject pointerIcon;          //조준점 아이콘 오브젝트
    public Image iconImage;                 //아이콘 이미지 컴포넌트
    public Sprite[] iconSprites;            //아이콘 스프라이트

    private GameManager gameManager;
    [SerializeField]
    private CinemachineCamera playerCam;

    void Start()
    {
        gameManager = GameManager.instance;

        if (reticleText.activeSelf == true)
        {
            reticleText.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        DetectItem();
    }

    //Ray로 아이템 감지, currentItem에 임시 저장
    private void DetectItem()
    {
        //Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (currentItem != hit.collider.gameObject)
            {
                currentItem = hit.collider.gameObject;
                isReticalOnItem = true;

                if (currentItem.CompareTag("Item"))
                {
                    ShowInteractUI("item", true);
                    Debug.Log("아이템 포착" + currentItem.name);
                }
                else if (currentItem.CompareTag("Openable"))
                {
                    ShowInteractUI("Openable", true);
                    Debug.Log("문 또는 서랍 포착" + currentItem.name);
                }
                else if (currentItem.CompareTag("Interactable"))
                {
                    ShowInteractUI("Interactable", true);
                }
                else
                {
                    CrearCurrentItem();
                }
            }
        }
        else
        {
            CrearCurrentItem();
        }
    }

    //오브젝트에 따른 아이콘 변경
    private void ShowInteractUI(string kind, bool state)
    {
        if (kind == "item")
        {
            iconImage.sprite = iconSprites[0];
        }
        else if (kind == "Openable")
        {
            iconImage.sprite = iconSprites[1];
        }
        else if (kind == "Interactable")
        {
            iconImage.sprite = iconSprites[2];
        }
        pointerIcon.SetActive(!state);
        reticleText.SetActive(state);
        iconImage.gameObject.SetActive(state);
    }

    //현재 조준중인 아이템이 없을 경우 초기화 하는 메소드
    private void CrearCurrentItem()
    {
        if (currentItem != null)
        {
            currentItem = null;
            isReticalOnItem = false;
            pointerIcon.SetActive(true);
            reticleText.SetActive(false);
            iconImage.gameObject.SetActive(false);
        }
    }

    //상호작용 확인
    public void InteractionCheck()
    {
        if (currentItem != null)
        {
            //Debug.Log("확인");
            if (currentItem.CompareTag("Item")) //선택된 오브젝트가 아이탬 태그일 때
            {
                
                var _itemDataSet = currentItem.GetComponent<ItemDataSet>();
                if (_itemDataSet == null) return;

                switch (_itemDataSet.thisItemData.itemType)
                {
                    case ItemType.item:
                        gameManager.inventoryManager.GetItem(currentItem);
                        break;
                    case ItemType.notePage:
                        gameManager.guideNoteManager.AddHintNote(_itemDataSet);
                        break;
                    default:
                        Debug.Log("아이템 타입이 지정되지 않음");
                        break;
                }
            }
            else if (currentItem.CompareTag("Openable")) //선택된 오브젝트가 열기 가능한 태그일 때
            {
                //Animator _currentDoorAnim = currentItem.GetComponent<Animator>();
                //IInteractable interactable = currentItem.GetComponentInParent<IInteractable>();

                /*if (interactable != null) //IInteractable를 가지고 있을 경우
                {
                    interactable.Interact();
                }*/
                OpenableSet openableSet = currentItem.GetComponentInParent<OpenableSet>();
                LockedObject lockedObject = currentItem.GetComponentInParent<LockedObject>();

                if (openableSet != null)
                {
                    openableSet.OpenableCheck(currentItem);
                }
                else if (lockedObject != null)
                {
                    lockedObject.CheckInventory();
                }
                else
                {
                    Animator _currentDoorAnim = currentItem.GetComponentInParent<Animator>();
                    DoorActive(_currentDoorAnim, currentItem);
                }
            }
            else if (currentItem.CompareTag("Interactable"))
            {
                IInteractable interactable = currentItem.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }
    }

    //임시 테스트용 문 여는 메소드
    public void DoorActive(Animator _animator, GameObject doorObject)
    {
        //Vector3 directionToPlayer = (this.gameObject.transform.position - doorObject.transform.position).normalized;
        Vector3 localPlayerPos = doorObject.transform.InverseTransformPoint(this.gameObject.transform.position);

        if ((_animator.GetBool("DoorOpenNeg")) == true || (_animator.GetBool("DoorOpenPos")) == true)
        {
            _animator.SetBool("DoorOpenNeg", false);
            _animator.SetBool("DoorOpenPos", false);
            AudioManager.Instance.PlaySFX("DoorClose", doorObject.transform.position);
            return;
        }

        if (localPlayerPos.x > 0 /*Vector3.Dot(directionToPlayer, transform.right) > 0*/) //문 오브젝트의 회전값 문제로 인해 임시 forward
        {
            _animator.SetBool("DoorOpenNeg", true);
            AudioManager.Instance.PlaySFX("DoorOpen", doorObject.transform.position);
        }
        else
        {
            _animator.SetBool("DoorOpenPos", true);
            AudioManager.Instance.PlaySFX("DoorOpen", doorObject.transform.position);
        }
    }

    public void DrawerActive()
    {

    }
}
