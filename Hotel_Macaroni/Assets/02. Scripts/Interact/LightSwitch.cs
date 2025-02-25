using UnityEngine;

public class LightSwitch : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject lightObject;

    public void Interact()
    {
        bool isActive = !lightObject.activeSelf;
        if (isActive)
        {

        }
        else
        {

        }

        lightObject.SetActive(isActive);
        AudioManager.Instance.PlaySFX("SwitchOff", this.transform.position);
    }
}
