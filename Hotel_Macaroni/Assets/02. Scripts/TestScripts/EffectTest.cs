using UnityEngine;

public class EffectTest : MonoBehaviour
{
    public EffectManager effectManager;

    private void OnTriggerEnter(Collider other)
    {
        if(effectManager != null && other.CompareTag("Player"))
        {
            effectManager.GlitchScreenEffect();
            Debug.Log("플레이어가 이펙트 지역에 진입");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어가 이펙트 지역서 나옴");
        }
    }
}
