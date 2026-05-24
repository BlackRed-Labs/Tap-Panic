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
    private bool isDestroyed = false; // Prevent double destruction

    private Color color;
    int score;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = GameManager.Instance;
        difficultyManager = DifficultyManager.Instance;
    }

    private void Start()
    {
        newBestWindow = gameManager.BestTime;
        survivedTime = GameManager.Instance != null ? GameManager.Instance.survivalTime : 0f;
        bestTime = PlayerPrefs.GetFloat("BestTime", 0f);
        score = gameManager.score;
        
        // Set ballLifeSpan based on difficulty tier
        ballLifeSpan = GetBallLifeSpanByScore(score);
        
        StartCoroutine(BallLifeSpanCoroutine());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var color = spriteRenderer != null ? spriteRenderer.color : Color.white;

        if (GetInstanceID() < collision.gameObject.GetInstanceID()) {
            AudioManager.Instance?.BallBounceSFX();
        }
    }

    private void OnMouseDown()
    {
        // Prevent destruction if game is paused or already destroyed
        if (gameManager != null && Time.timeScale > 0 && !isDestroyed)
        {
            DestroyTheBall();
        }
    }

    public void DestroyTheBall()
    {
        if (isDestroyed) return; // Prevent double destruction
        
        isDestroyed = true;
        LevelSelect();
        gameManager?.ScoreSystem();
        color = spriteRenderer != null ? spriteRenderer.color : Color.white;
        gameManager?.destroyEffect(color, transform.position);
        AudioManager.Instance?.BallDestroySFX();
        Destroy(gameObject);
    }

    /// <summary>
    /// Determines the ball lifespan based on the current score tier
    /// </summary>
    private float GetBallLifeSpanByScore(int currentScore)
    {
        if (currentScore <= 500)
        {
            return 5f; // Beginner Level
        }
        else if (currentScore <= 2000)
        {
            return 4f; // Intermediate Level (501-2000)
        }
        else if (currentScore <= 4000)
        {
            return 3f; // Intermediate Level 2 & Advance Level (2001-4000)
        }
        else
        {
            return 2f; // Master Level (4001+)
        }
    }

    private void LevelSelect()
    {
        if (gameManager == null || difficultyManager == null) return;

        // Update score in case it changed
        score = gameManager.score;

        // ===== BEGINNER LEVEL (score 0-500) =====
        if (score <= 10)
        {
            difficultyManager.LevelOne();
            Debug.Log("Level 1: Score 0-10");
        }
        else if (score <= 30)
        {
            difficultyManager.LevelTwo();
            Debug.Log("Level 2: Score 11-30");
        }
        else if (score <= 100)
        {
            difficultyManager.LevelThree();
            Debug.Log("Level 3: Score 31-100");
        }
        
        // ===== INTERMEDIATE LEVEL (score 101-500) =====
        else if (score <= 150)
        {
            if (score == 101 && !difficultyManager.is2BallSpawned)
            {
                // Spawn 3 balls at Level 4 start
                for (int i = 0; i < 2; i++)
                    difficultyManager.LevelFour();
                difficultyManager.is2BallSpawned = true;
            }
            else if (difficultyManager.is2BallSpawned)
            {
                difficultyManager.LevelFour();
            }
            Debug.Log("Level 4: Score 101-150");
        }
        else if (score <= 250)
        {
            if (score == 151 && !difficultyManager.is3BallSpawned)
            {
                // Spawn 1 health buff ball at Level 5 start
                difficultyManager.LevelFive();
                difficultyManager.is3BallSpawned = true;
            }
            else if (difficultyManager.is3BallSpawned)
            {
                difficultyManager.LevelFive();
            }
            Debug.Log("Level 5: Score 151-250");
        }
        else if (score <= 350)
        {
            difficultyManager.LevelSix();
            Debug.Log("Level 6: Score 251-350");
        }
        else if (score <= 500)
        {
            difficultyManager.LevelSeven();
            Debug.Log("Level 7: Score 351-500");
        }
        
        // ===== INTERMEDIATE LEVEL 2 (score 501-2000, Ball self destroy time = 3f) =====
        else if (score <= 700)
        {
            if (score == 501 && !difficultyManager.is4BallSpawned)
            {
                // Spawn 4 balls at Level 8 start
                for (int i = 0; i < 4; i++)
                    difficultyManager.LevelEight();
                difficultyManager.is4BallSpawned = true;
            }
            else if (difficultyManager.is4BallSpawned)
            {
                difficultyManager.LevelEight();
            }
            Debug.Log("Level 8: Score 501-700");
        }
        else if (score <= 1000)
        {
            if (score == 701 && !difficultyManager.is5BallSpawned)
            {
                // Spawn 2 health buff balls at Level 9 start
                for (int i = 0; i < 2; i++)
                    difficultyManager.LevelNine();
                difficultyManager.is5BallSpawned = true;
            }
            else if (difficultyManager.is5BallSpawned)
            {
                difficultyManager.LevelNine();
            }
            Debug.Log("Level 9: Score 701-1000");
        }
        else if (score <= 1500)
        {
            difficultyManager.LevelTen();
            Debug.Log("Level 10: Score 1001-1500");
        }
        else if (score <= 2000)
        {
            difficultyManager.LevelEleven();
            Debug.Log("Level 11: Score 1501-2000");
        }

        // ===== ADVANCE LEVEL (score 2001-4000, Ball self destroy time = 2f) =====
        else if (score <= 2500)
        {
            if (score == 2001 && !difficultyManager.isBombSpawned)
            {
                // Spawn 5 balls at Level 12 start
                for (int i = 0; i < 5; i++)
                    difficultyManager.LevelTwelve();
                difficultyManager.isBombSpawned = true;
            }
            else if (difficultyManager.isBombSpawned)
            {
                difficultyManager.LevelTwelve();
            }
            Debug.Log("Level 12: Score 2001-2500");
        }
        else if (score <= 3000)
        {
            difficultyManager.LevelThirteen();
            Debug.Log("Level 13: Score 2501-3000");
        }
        else if (score <= 3500)
        {
            difficultyManager.LevelFourteen();
            Debug.Log("Level 14: Score 3001-3500");
        }
        else if (score <= 4000)
        {
            difficultyManager.LevelFifteen();
            Debug.Log("Level 15: Score 3501-4000");
        }
        
        // ===== MASTER LEVEL (score 4001+, Ball self destroy time = 2f) =====
        else if (score >= 4001)
        {
            difficultyManager.LevelSixteen();
            Debug.Log("Level 16: Score 4001+");
        }
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
        if (gameManager != null && !isDestroyed)
        {
            gameManager.HealthSystem();

            if (gameManager.Health <= 0)
            {
                gameManager.Revive();
            }

            LevelSelect();
            gameManager.destroyEffect(color, transform.position);
            AudioManager.Instance?.BallDestroySFX();
        }
        
        Destroy(gameObject);
    }
}


