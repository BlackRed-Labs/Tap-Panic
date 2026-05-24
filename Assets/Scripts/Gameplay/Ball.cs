using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    // Cached refs
    private GameManager gameManager;
    private DifficultyManager difficultyManager;
    private SpriteRenderer spriteRenderer;

    // Config
    [SerializeField] private float ballLifeSpan = 5f;

    // State
    private float survivedTime;
    private float bestTime;
    private GameObject newBestWindow;

    private Color color;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = GameManager.Instance != null ? GameManager.Instance : FindAnyObjectByType<GameManager>();
        difficultyManager = FindAnyObjectByType<DifficultyManager>();
    }

    private void Start()
    {
        newBestWindow = gameManager.BestTime;
        survivedTime = GameManager.Instance != null ? GameManager.Instance.survivalTime : 0f;
        bestTime = PlayerPrefs.GetFloat("BestTime", 0f);

        StartCoroutine(BallLifeSpanCoroutine());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        var color = spriteRenderer != null ? spriteRenderer.color : Color.white;

        //gameManager?.BallBounceEffect(color, hitPoint);
        if (GetInstanceID() < collision.gameObject.GetInstanceID()) {
            AudioManager.Instance?.BallBounceSFX();
        }
            
    }

    private void OnMouseDown()
    {
        
        if (gameManager != null)
         
           DestroyTheBall();
    }

    public void DestroyTheBall()
    {
        LevelSelect();
        gameManager?.ScoreSystem();
         color = spriteRenderer != null ? spriteRenderer.color : Color.white;
        gameManager?.destroyEffect(color, transform.position);
        AudioManager.Instance?.BallDestroySFX();

        Destroy(gameObject);
    }

    private void LevelSelect()
    {
        if (gameManager == null || difficultyManager == null) return;

        int score = gameManager.score;

        // Use a more efficient approach with score ranges
        if (score <= 10)
        {
            difficultyManager.LevelOne();
            Debug.Log("Level 1");
        }
        else if (score <= 20)
        {
            difficultyManager.LevelTwo();
                Debug.Log("Level 2");
        }
        else if (score <= 100)
        {
            difficultyManager.LevelThree();
                Debug.Log("Level 3");
        }
        else if (score > 100 && !difficultyManager.is2BallSpawned)
        {
            Debug.Log("Level 4");
            for (int i = 0; i < 2; i++)
                difficultyManager.LevelFour();
            difficultyManager.is2BallSpawned = true;
        }
        else if (score <= 300 && difficultyManager.is2BallSpawned)
            difficultyManager.LevelFour();
        else if (score > 300 && !difficultyManager.is3BallSpawned)
        {
            for (int i = 0; i < 2; i++)
                difficultyManager.LevelFive();
            difficultyManager.is3BallSpawned = true;
        }
        else if (score < 450 && difficultyManager.is3BallSpawned)
            difficultyManager.LevelFive();
        else if (score <= 550)
            difficultyManager.LevelSix();
        else if (score > 550 && !difficultyManager.is4BallSpawned)
        {
            for (int i = 0; i < 3; i++)
                difficultyManager.LevelSix();
            difficultyManager.is4BallSpawned = true;
        }
        else if (score < 700 && difficultyManager.is4BallSpawned)
            difficultyManager.LevelSeven();
        else if (score < 1000)
            difficultyManager.LevelEight();
        else if (score < 2000 && !difficultyManager.is5BallSpawned && score > 1000)
        {
            for (int i = 0; i < 4; i++)
                difficultyManager.LevelNine();
            difficultyManager.is5BallSpawned = true;
        }
        else if (score < 2000 && difficultyManager.is4BallSpawned && score > 1000)
            difficultyManager.LevelNine();
    }

    private IEnumerator BallLifeSpanCoroutine()
    {
        float elapsed = 0f;
        // Create gradient Green -> Yellow -> Red for more efficient color transitions
        Gradient colorGradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[3];
        colorKeys[0].color = Color.white;
        colorKeys[0].time = 0f;
        colorKeys[1].color = Color.white;
        colorKeys[1].time = 0.5f;
        colorKeys[2].color = Color.red;
        colorKeys[2].time = 1f;
        colorGradient.colorKeys = colorKeys;

        while (elapsed < ballLifeSpan)
        {
            float t = elapsed / ballLifeSpan;
            if (spriteRenderer != null)
                spriteRenderer.color = colorGradient.Evaluate(t);

            elapsed += Time.deltaTime;
            yield return null;
        }


        // Apply health / end-game logic before destroying this GameObject
        if (gameManager != null)
        {
            gameManager.HealthSystem();

            if (gameManager.Health <= 0)
            {
               
                // Only trigger revive if health is actually <= 0
                gameManager.Revive();
            }

            LevelSelect();
        }
        gameManager.destroyEffect(color, transform.position);
        AudioManager.Instance?.BallDestroySFX();
        Destroy(gameObject);

    }
}


