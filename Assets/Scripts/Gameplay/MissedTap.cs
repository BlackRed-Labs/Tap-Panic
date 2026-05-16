using UnityEngine;

public class MissedTap : MonoBehaviour
{
    public GameManager GameManager;

    private void OnMouseDown()
    {
        if (!gameObject.CompareTag("Tap Missed")) return;

        if (GameManager.Health > 0 && GameManager != null && gameObject.CompareTag("Tap Missed"))
        {

            GameManager.HealthSystem();
            

            // If health reached zero after decrement, trigger Revive immediately
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
