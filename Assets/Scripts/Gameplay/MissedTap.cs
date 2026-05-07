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
        if (!gameObject.CompareTag("Tap Missed")) return;

        if (GameManager.Health > 0)
        {
            GameManager.HealthSystem();

            // If health reached zero after decrement, trigger game over immediately
            if (GameManager.Health <= 0)
            {
                GameManager.Revive();
            }
        }
        else
        {
            // If health was already zero or below, ensure game over runs
            GameManager.GameOver();
        }
    }
}
