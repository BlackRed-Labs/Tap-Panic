using UnityEngine;

public class HeartBall : MonoBehaviour
{
     private GameManager gameManager;
     private UIManager uiManager;
    private bool isDestroyed = false;

    private void Start()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }
        if (uiManager == null)
        {
            uiManager = UIManager.Instance;
        }
    }

    private void OnMouseDown()
    {
            if (!isDestroyed && gameManager != null && uiManager != null && gameManager.Health < 5 && Time.timeScale > 0)
            {
            isDestroyed = true;
            gameManager.Health++; // Increment first
            uiManager.AddLife(gameManager.Health); // Then update UI with new health (1-5)
            Color heartColor = gameObject.GetComponent<SpriteRenderer>().color;
            gameManager.destroyEffect(heartColor, transform.position);
            gameManager.AddHealthEffect(transform.position);
            AudioManager.Instance.PlayHealthCollected();
            Destroy(gameObject);
            }
    }
}
