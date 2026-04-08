using UnityEngine;

public class HeartBall : MonoBehaviour
{
    GameManager GameManager;
    UIManager UIManager;

    int HealthCount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager = FindAnyObjectByType<GameManager>();
        UIManager = FindAnyObjectByType<UIManager>();
        HealthCount = GameManager.Health;
        
    }

    private void OnMouseDown()
    {
        if (HealthCount < 5)
        {
            HealthCount++;
            UIManager.AddLife(HealthCount);
          
        }
        Destroy(gameObject);
    }

}
