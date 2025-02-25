using UnityEngine;

public class TestDeadEvent : MonoBehaviour
{
    private GameManager gameManager;
    public int count = 0;
    private void Start()
    {
        gameManager = GameManager.instance;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            count++;
            if(count == 1)
            {
                gameManager.eventManager.NowActiveEvent(this.gameObject);
            }
            else if(count > 1)
            {
                gameManager.eventManager.EventFailed("7F000");
            }
        }
    }
}
