using System.Collections;
using UnityEngine;

public class DoNotRun : MonoBehaviour
{
    public bool isDebuffOn = false;
    private float debuffTime = 10f;

    public void RandomDebuffOn()
    {
        if (isDebuffOn == true) return;

        int randomValue = Random.Range(0, 100);
        if (randomValue < 40)
        {
            StartCoroutine(PlayerSlowDebuff());
        } 
        else if (randomValue < 80)
        {
            //StartCoroutine(PlayerEyeDebuff());
        }
        else
        {
            DeadDebuff();
        }

        

    }

    private IEnumerator PlayerSlowDebuff()
    {
        GameManager.instance.playerController.PlayerMoveSpeed(2, 5);
        yield return new WaitForSeconds(10f);
        GameManager.instance.playerController.PlayerMoveSpeed(5, 10);
    }

    /*private IEnumerator PlayerEyeDebuff()
    {

    }*/

    private void DeadDebuff()
    {

    }
}
