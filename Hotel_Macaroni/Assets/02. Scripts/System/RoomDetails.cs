using UnityEngine;

public class RoomDetails : MonoBehaviour
{
    [SerializeField] private string roomNum;
    public bool isDoorOpen; //문 열렸는지 닫혔는지
    public bool isPlayerIn; //플레이어 있는지 없는지
    public bool isLightOn;  //조명 켰는지 껏는지

    [SerializeField] private GameObject roomLight;
    [SerializeField] private Animator doorAnimator;

    private void Start()
    {
        //isLightOn = roomLight.activeSelf;
        
        if(doorAnimator != null)
        {
            if (doorAnimator.GetBool("DoorOpenNeg") == true || (doorAnimator.GetBool("DoorOpenPos")) == true)
            {
                isDoorOpen = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerIn = true;
            Debug.Log("Player" + roomNum + "호 진입");
            //EventManager.Instance.
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerIn = false;
        }
    }

    public void RoomLightSwitch(bool state)
    {
        //bool isActive = !roomLight.activeSelf;
        //roomLight.SetActive(isActive);
        //AudioManager.Instance.PlaySFX("SwitchOff", this.transform.position);
        isLightOn = state;
    }

    
}
