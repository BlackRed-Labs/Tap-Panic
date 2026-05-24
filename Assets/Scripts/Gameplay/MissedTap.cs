using UnityEngine;

public class MissedTap : MonoBehaviour
{
    public GameManager GameManager;
    [SerializeField] private GameObject MouseclickEffectPrefab;
    private bool isClicked = false; // Prevent multiple clicks in same frame

    private void OnMouseDown()
    {
        // Guard clauses to prevent issues
        if (isClicked || GameManager == null) return;
        if (!gameObject.CompareTag("Tap Missed")) return;
        if (GameManager.Health <= 0) return; // Already dead, don't process
        if (Time.timeScale == 0) return; // Game is paused

        isClicked = true;
        
        GameManager.HealthSystem();
        SpawnClickEffect();

        // If health reached zero after decrement, trigger Revive immediately
        if (GameManager.Health <= 0)
        {
            GameManager.Revive();
        }

        // Reset for next frame
        Invoke(nameof(ResetClick), 0.05f);
    }

    private void ResetClick()
    {
        isClicked = false;
    }

    private void SpawnClickEffect()
    {
        // Only spawn effect if time is running
        if (Time.timeScale <= 0) return;

        Vector3 effectPos = Input.mousePosition;
        effectPos.z = Mathf.Abs(Camera.main.transform.position.z);

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(effectPos);
        worldPos.z = 0f;

        GameObject effect = Instantiate(MouseclickEffectPrefab, worldPos, Quaternion.identity);
    }
}

