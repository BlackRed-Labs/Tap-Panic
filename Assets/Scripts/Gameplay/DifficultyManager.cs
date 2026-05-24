using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class DifficultyManager : MonoBehaviour
{
   public static DifficultyManager Instance { get; private set; } = null!;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Header("References")]
    public GameManager gameManager;

    [Header("Special Ball Prefabs")]
    public GameObject heartBallPrefab;  // Heart ball for rewards
    public GameObject healthBuffBallPrefab;  // Health buff ball

    [Header("Ball settings")]
    public float BallForce = 3f;
    public float CurrectScaleOfBall = 1.5f;

    [Header("Spawn flags & timing")]
    [HideInInspector] public bool isBombSpawned = false;
    [HideInInspector] public bool is2BallSpawned = false;
    [HideInInspector] public bool is3BallSpawned = false;
    [HideInInspector] public bool is4BallSpawned = false;
    [HideInInspector] public bool is5BallSpawned = false;
    
    [Header("Level milestone flags")]
    [HideInInspector] public bool isLevel4Started = false;
    [HideInInspector] public bool isLevel8Started = false;
    [HideInInspector] public bool isLevel12Started = false;

    

    // Utility: spawn a prefab via GameManager (keeps spawn logic centralized)
    private GameObject SpawnBallFromManager()
    {
        return gameManager != null ? gameManager.SpawnBall() : null;
    }

    // Utility: random left/right direction for ball spawning
    private int RandomDirection()
    {
        return Random.value < 0.5f ? -1 : 1;
    }

    //Apply initial force to the ball's Rigidbody2D, with random left/right direction
    private void ApplyInitialForce(Rigidbody2D rb, float force)
    {
        int dir = RandomDirection();
        rb.AddForce(new Vector2(dir * force, 0f), ForceMode2D.Impulse);
    }

    // Spawns a ball, applies scale, mass, damping, and gravity, and gives it an initial force
    private GameObject SpawnAndConfigureBall(float scale, int mass = 3, float linearDamping = 0f, float angularDamping = 0f, float gravityScale = 1f)
    {
        var ball = SpawnBallFromManager();
        if (ball == null) return null;

        ball.transform.localScale = Vector3.one * scale;

        var rb = ball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.mass = mass;
            rb.linearDamping = linearDamping;
            rb.angularDamping = angularDamping;
            rb.gravityScale = gravityScale;
        }

        ApplyInitialForce(rb, BallForce);
        return ball;
    }

    /// <summary>
    /// Spawns a special ball (heart or health buff) at a random position
    /// </summary>
    private GameObject SpawnSpecialBall(GameObject prefab)
    {
        if (prefab == null) return null;

        Vector2 spawnPosition = new Vector2(Random.Range(-0.16f, 2.5f), Random.Range(0f, 1.8f));
        GameObject specialBall = Instantiate(prefab, spawnPosition, Quaternion.identity);
        specialBall.transform.localScale = Vector3.one ; // Set a default scale for special balls

        return specialBall;
    }

    /// <summary>
    /// Increases bounciness of a ball by creating/modifying its PhysicsMaterial2D
    /// </summary>
    private void IncreaseBounciness(GameObject ball, float bouncinessIncrease = 0.1f)
    {
        if (ball == null) return;

        var collider = ball.GetComponent<CircleCollider2D>();
        if (collider == null) return;

        // Get or create a new physics material
        PhysicsMaterial2D physicsMaterial = collider.sharedMaterial;
        
        if (physicsMaterial == null)
        {
            // Create a new physics material if none exists
            physicsMaterial = new PhysicsMaterial2D();
            physicsMaterial.name = "BouncyMaterial";
        }
        else
        {
            // Clone the material to avoid modifying the original
            physicsMaterial = Instantiate(physicsMaterial);
        }

        // Increase bounciness
        physicsMaterial.bounciness = Mathf.Clamp(physicsMaterial.bounciness + bouncinessIncrease, 0f, 1f);
        collider.sharedMaterial = physicsMaterial;
    }

    #region Levels

    // ===== BEGINNER LEVEL (Ball self destroy time = 5f) =====
    
    // Level 1: score 0 to 10 -> Spawn 2 balls
    // Ball size 1.8, Mass = 3, Linear damping = 1, Angular damping 1, Gravity = 1.8
    public void LevelOne()
    {
        BallForce = 3f;
        SpawnAndConfigureBall(1.8f, 5, 1f, 1f, 1.8f);
    }

    // Level 2: score 11 to 30 -> Add random directions to the ball
    // Ball size 1.8, Mass = 3, Linear damping = 1, Angular damping 1, Gravity = 1.8
    public void LevelTwo()
    {
        BallForce = 3f;
        SpawnAndConfigureBall(1.8f, 5, 1f, 1f, 1.8f);
    }

    // Level 3: score 31 to 100 -> Add extra force 0.01 to initial force (3f) clamp to 5f
    // Ball size 1.8 to 1.5, Mass = 3, Linear damping = 1, Angular damping 1, Gravity = 1.8
    public void LevelThree()
    {
        BallForce = Mathf.Clamp(BallForce + 0.01f, 3f, 5f);
        float scale = Mathf.Lerp(1.8f, 1.5f, (gameManager.score - 31f) / (100f - 31f));
        scale = Mathf.Clamp(scale, 1.5f, 1.8f);
        SpawnAndConfigureBall(scale, 5, 1f, 1f, 1.8f);
    }

    // ===== INTERMEDIATE LEVEL (Ball self destroy time = 4f) =====
    
    // Level 4: score 100 to 150 -> 1 Heart ball as reward + Spawn 3 balls
    // Ball size 1.5, Mass = 4, Linear damping = 1.2, Angular damping 1.2, Gravity = 1.9
    public void LevelFour()
    {
        BallForce = 3f;

        GameObject ball = SpawnAndConfigureBall(1.5f, 7, 1.3f, 1.3f, 1.9f);

        // Spawn heart ball as reward once at level start (score 101)
        if (!isLevel4Started && gameManager.score >= 101)
        {
            GameObject heartBall = SpawnSpecialBall(heartBallPrefab);
            if (heartBall != null) { 
              heartBall.transform.localScale = ball.transform.localScale; // Match scale of regular balls
            }
            isLevel4Started = true;
            Debug.Log("Heart Ball Reward Spawned!");
        }

         
    }

    // Level 5: Score 151 to 250 -> Spawn 1 health buff ball + reduce ball size
    // Ball size 1.5 to 1.2, Mass = 4, Linear damping = 1.2, Angular damping 1.2, Gravity = 1.9
    public void LevelFive()
    {
        BallForce = 3f;
        
        float scale = Mathf.Lerp(1.5f, 1.2f, (gameManager.score - 151f) / (250f - 151f));
        scale = Mathf.Clamp(scale, 1.2f, 1.5f);
        
        // Spawn health buff ball at level start (score 151)
        if (gameManager.score == 151)
        {
            GameObject healthBuffBall = SpawnSpecialBall(healthBuffBallPrefab);
            if (healthBuffBall != null)
            {
                healthBuffBall.transform.localScale = Vector3.one * scale;
            }
            Debug.Log("Health Buff Ball Spawned!");
        }

        SpawnAndConfigureBall(scale, 7, 1.3f, 1.3f, 1.9f);
    }

    // Level 6: Score 251 to 350 -> Spawn 1 health buff ball + Add extra force 0.01 to initial force (5f) and clamp to 8f
    // Ball size 1.2, Mass = 4, Linear damping = 1.2, Angular damping 1.2, Gravity = 1.9
    public void LevelSix()
    {
        BallForce = Mathf.Clamp(BallForce + 0.01f, 5f, 8f);

        GameObject ball = SpawnAndConfigureBall(1.2f, 7, 1.3f, 1.3f, 1.9f);

        // Spawn health buff ball at level start (score 251)
        if (gameManager.score == 251)
        {
            GameObject healthBuffBall = SpawnSpecialBall(healthBuffBallPrefab);
            if (healthBuffBall != null)
            {
                healthBuffBall.transform.localScale = ball.transform.localScale;
            }
            Debug.Log("Health Buff Ball Spawned!");
        }

        
    }

    // Level 7: Score 351 to 500 -> Spawn 1 health buff ball + increase the bounciness of 0.1
    // Ball size 1.2, Mass = 4, Linear damping = 1.2, Angular damping 1.2, Gravity = 1.9
    public void LevelSeven()
    {
        BallForce = 3f;
        
        var ball = SpawnAndConfigureBall(1.2f, 7, 1.3f, 1.3f, 1.9f);
        
        // Spawn health buff ball at level start (score 351)
        if (gameManager.score == 351)
        {
            GameObject healthBuffBall = SpawnSpecialBall(healthBuffBallPrefab);
            if (healthBuffBall != null)
            {
                healthBuffBall.transform.localScale = ball.transform.localScale;
            }
            Debug.Log("Health Buff Ball Spawned!");
        }

        if (ball != null)
        {
            IncreaseBounciness(ball, 0.1f);
        }
    }

    // ===== INTERMEDIATE LEVEL 2 (Ball self destroy time = 3f) =====
    
    // Level 8: score 501 to 700 -> 2 Heart ball as reward + Spawn 4 balls
    // Ball size 1.2, Mass = 5, Linear damping = 1.3, Angular damping 1.3, Gravity = 2
    public void LevelEight()
    {
        BallForce = 3f;
        
        GameObject ball = SpawnAndConfigureBall(1.2f, 9, 1.4f, 1.4f, 2f);
        
        // Spawn 2 heart balls as reward once at level start (score 501)
        if (!isLevel8Started && gameManager.score >= 501)
        {
            GameObject heartBall1 = SpawnSpecialBall(heartBallPrefab);
            if (heartBall1 != null)
            {
                heartBall1.transform.localScale = ball.transform.localScale;
            }
            
            GameObject heartBall2 = SpawnSpecialBall(heartBallPrefab);
            if (heartBall2 != null)
            {
                heartBall2.transform.localScale = ball.transform.localScale;
            }
            
            isLevel8Started = true;
            Debug.Log("2 Heart Ball Rewards Spawned!");
        }
    }

    // Level 9: Score 701 to 1000 -> Spawn 2 health buff ball + reduce ball size
    // Ball size 1.2 to 1, Mass = 5, Linear damping = 1.3, Angular damping 1.3, Gravity = 2
    public void LevelNine()
    {
        BallForce = 3f;
        
        float scale = Mathf.Lerp(1.2f, 1f, (gameManager.score - 701f) / (1000f - 701f));
        scale = Mathf.Clamp(scale, 1f, 1.2f);
        
        // Spawn 2 health buff balls at level start (score 701)
        if (gameManager.score == 701)
        {
            GameObject healthBuffBall1 = SpawnSpecialBall(healthBuffBallPrefab);
            if (healthBuffBall1 != null)
            {
                healthBuffBall1.transform.localScale = Vector3.one * scale;
            }
            
            GameObject healthBuffBall2 = SpawnSpecialBall(healthBuffBallPrefab);
            if (healthBuffBall2 != null)
            {
                healthBuffBall2.transform.localScale = Vector3.one * scale;
            }
            
            Debug.Log("2 Health Buff Balls Spawned!");
        }

        SpawnAndConfigureBall(scale, 9, 1.4f, 1.4f, 2f);
    }

    // Level 10: Score 1001 to 1500 -> Spawn 2 health buff ball + Add extra force 0.01 to initial force (8f) and clamp to 10f
    // Ball size 1, Mass = 5, Linear damping = 1.3, Angular damping 1.3, Gravity = 2
    public void LevelTen()
    {
        BallForce = Mathf.Clamp(BallForce + 0.01f, 8f, 10f);
        
        GameObject ball = SpawnAndConfigureBall(1f, 9, 1.4f, 1.4f, 2f);
        
        // Spawn 2 health buff balls at level start (score 1001)
        if (gameManager.score == 1001)
        {
            GameObject healthBuffBall1 = SpawnSpecialBall(healthBuffBallPrefab);
            if (healthBuffBall1 != null)
            {
                healthBuffBall1.transform.localScale = ball.transform.localScale;
            }
            
            GameObject healthBuffBall2 = SpawnSpecialBall(healthBuffBallPrefab);
            if (healthBuffBall2 != null)
            {
                healthBuffBall2.transform.localScale = ball.transform.localScale;
            }
            
            Debug.Log("2 Health Buff Balls Spawned!");
        }
    }

    // Level 11: Score 1501 to 2000 -> Spawn 2 health buff ball + increase the bounciness of 0.1
    // Ball size 1.2, Mass = 5, Linear damping = 1.3, Angular damping 1.3, Gravity = 2
    public void LevelEleven()
    {
        BallForce = 3f;
        
        var ball = SpawnAndConfigureBall(1.2f, 9, 1.4f, 1.4f, 2f);
        
        // Spawn 2 health buff balls at level start (score 1501)
        if (gameManager.score == 1501)
        {
            GameObject healthBuffBall1 = SpawnSpecialBall(healthBuffBallPrefab);
            if (healthBuffBall1 != null)
            {
                healthBuffBall1.transform.localScale = ball.transform.localScale;
            }
            
            GameObject healthBuffBall2 = SpawnSpecialBall(healthBuffBallPrefab);
            if (healthBuffBall2 != null)
            {
                healthBuffBall2.transform.localScale = ball.transform.localScale;
            }
            
            Debug.Log("2 Health Buff Balls Spawned!");
        }
        
        if (ball != null)
        {
            IncreaseBounciness(ball, 0.1f);
        }
    }

    // ===== ADVANCE LEVEL (Ball self destroy time = 2f) =====
    
    // Level 12: score 2001 to 2500 -> 3 Heart ball as reward + Spawn 5 balls
    // Ball size 1, Mass = 6, Linear damping = 1.4, Angular damping 1.4, Gravity = 2.1
    public void LevelTwelve()
    {
        BallForce = 3f;
        
        GameObject ball = SpawnAndConfigureBall(1f, 10, 1.5f, 1.5f, 2.1f);
        
        // Spawn 3 heart balls as reward once at level start (score 2001)
        if (!isLevel12Started && gameManager.score >= 2001)
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject heartBall = SpawnSpecialBall(heartBallPrefab);
                if (heartBall != null)
                {
                    heartBall.transform.localScale = ball.transform.localScale;
                }
            }
            isLevel12Started = true;
            Debug.Log("3 Heart Ball Rewards Spawned!");
        }
    }

    // Level 13: Score 2501 to 3000 -> Spawn 3 health buff ball + reduce ball size
    // Ball size 1 to 0.8, Mass = 6, Linear damping = 1.4, Angular damping 1.4, Gravity = 2.1
    public void LevelThirteen()
    {
        BallForce = 3f;
        
        float scale = Mathf.Lerp(1f, 0.8f, (gameManager.score - 2501f) / (3000f - 2501f));
        scale = Mathf.Clamp(scale, 0.8f, 1f);
        
        // Spawn 3 health buff balls at level start (score 2501)
        if (gameManager.score == 2501)
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject healthBuffBall = SpawnSpecialBall(healthBuffBallPrefab);
                if (healthBuffBall != null)
                {
                    healthBuffBall.transform.localScale = Vector3.one * scale;
                }
            }
            Debug.Log("3 Health Buff Balls Spawned!");
        }

        SpawnAndConfigureBall(scale, 10, 1.5f, 1.5f, 2.1f);
    }

    // Level 14: Score 3001 to 3500 -> Spawn 3 health buff ball + Add extra force 0.01 to initial force (10f) and clamp to 12f
    // Ball size 0.8, Mass = 6, Linear damping = 1.4, Angular damping 1.4, Gravity = 2.1
    public void LevelFourteen()
    {
        BallForce = Mathf.Clamp(BallForce + 0.01f, 10f, 12f);
        
        GameObject ball = SpawnAndConfigureBall(0.8f, 10, 1.5f, 1.5f, 2.1f);
        
        // Spawn 3 health buff balls at level start (score 3001)
        if (gameManager.score == 3001)
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject healthBuffBall = SpawnSpecialBall(healthBuffBallPrefab);
                if (healthBuffBall != null)
                {
                    healthBuffBall.transform.localScale = ball.transform.localScale;
                }
            }
            Debug.Log("3 Health Buff Balls Spawned!");
        }
    }

    // Level 15: Score 3501 to 4000 -> Spawn 3 health buff ball + increase the bounciness of 0.1
    // Ball size 0.8, Mass = 6, Linear damping = 1.4, Angular damping 1.4, Gravity = 2.1
    public void LevelFifteen()
    {
        BallForce = 3f;
        
        var ball = SpawnAndConfigureBall(0.8f, 10, 1.5f, 1.5f, 2.1f);
        
        // Spawn 3 health buff balls at level start (score 3501)
        if (gameManager.score == 3501)
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject healthBuffBall = SpawnSpecialBall(healthBuffBallPrefab);
                if (healthBuffBall != null)
                {
                    healthBuffBall.transform.localScale = ball.transform.localScale;
                }
            }
            Debug.Log("3 Health Buff Balls Spawned!");
        }
        
        if (ball != null)
        {
            IncreaseBounciness(ball, 0.1f);
        }
    }

    // ===== MASTER LEVEL (Ball self destroy time = 2f) =====
    
    // Level 16: Score 4001 to 10000 -> Spawn 3 health buff ball on every 1000
    // Ball size 0.5, Mass = 7, Linear damping = 1.5, Angular damping 1.5, Gravity = 2.2
    public void LevelSixteen()
    {
        BallForce = 3f;
        
        GameObject ball = SpawnAndConfigureBall(0.5f, 10, 1.5f, 1.5f, 2.2f);
        
        // Spawn 3 health buff balls every 1000 score points
        if (gameManager.score >= 4001 && gameManager.score % 1000 == 0 && gameManager.score != 0)
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject healthBuffBall = SpawnSpecialBall(healthBuffBallPrefab);
                if (healthBuffBall != null)
                {
                    healthBuffBall.transform.localScale = ball.transform.localScale;
                }
            }
            Debug.Log("3 Health Buff Balls Spawned at score: " + gameManager.score);
        }
    }

    #endregion

    /// <summary>
    /// Resets all milestone flags when the game restarts
    /// </summary>
    public void ResetMilestoneFlags()
    {
        isLevel4Started = false;
        isLevel8Started = false;
        isLevel12Started = false;
        is2BallSpawned = false;
        is3BallSpawned = false;
        is4BallSpawned = false;
        is5BallSpawned = false;
        isBombSpawned = false;
    }
}
