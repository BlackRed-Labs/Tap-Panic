using UnityEngine;

public class MissedTap : MonoBehaviour
{
    public GameManager GameManager;

    private void Start()
    {
        GameManager = FindAnyObjectByType<GameManager>();
    }

    private void OnMouseDown()
    {
        if (gameObject.CompareTag("Tap Missed")) 
        {
            GameManager.HealthSystem();
        }
    }
}
