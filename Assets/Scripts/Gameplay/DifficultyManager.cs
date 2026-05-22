using System.Collections;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    [Header("References")]
    public GameManager gameManager;

    [Header("Ball settings")]
    public float BallForce = 3f;
    public float CurrectScaleOfBall = 1.5f;

    [Header("Spawn flags & timing")]
    [HideInInspector] public bool isBombSpawned = false;
    [HideInInspector] public bool is2BallSpawned = false;
    [HideInInspector] public bool is3BallSpawned = false;
    [HideInInspector] public bool is4BallSpawned = false;
    [HideInInspector] public bool is5BallSpawned = false;

    // Utility: spawn a prefab via GameManager (keeps spawn logic centralized)
    private GameObject SpawnBallFromManager()
    {
        return gameManager != null ? gameManager.SpawnBall() : null;
    }

    private int RandomDirection()
    {
        // faster and clearer 50/50 direction
        return Random.value < 0.5f ? -1 : 1;
    }

    private void ApplyInitialForce(Rigidbody2D rb, float force)
    {
        if (rb == null) return;
        if (gameManager != null && gameManager.isBonusScoreActive)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            return;
        }

        rb.bodyType = RigidbodyType2D.Dynamic;
        int dir = RandomDirection();
        rb.AddForce(new Vector2(dir * force, 0f), ForceMode2D.Impulse);
    }

    private GameObject SpawnAndConfigureBall(float scale, float mass = 1f, float linearDamping = 0.2f)
    {
        var ball = SpawnBallFromManager();
        if (ball == null) return null;

        ball.transform.localScale = Vector3.one * scale;

        var rb = ball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.mass = mass;
            rb.linearDamping = linearDamping;
        }

        ApplyInitialForce(rb, BallForce);
        return ball;
    }

    

    #region Levels

    public void LevelOne()
    {
        SpawnBallFromManager();
    }

    public void LevelTwo()
    {
        var ball = SpawnAndConfigureBall(CurrectScaleOfBall);
        // If bonus score active, ApplyInitialForce already made body kinematic
    }

    public void LevelThree()
    {
        BallForce = Mathf.Clamp(BallForce + 0.1f, 3f, 5f);
        SpawnAndConfigureBall(CurrectScaleOfBall);
    }

    public void LevelFour()
    {
        int score = gameManager != null ? gameManager.score : 0;
        if (score > 140 && score < 300)
        {
            BallForce = Mathf.Clamp(BallForce + 0.25f, 3f, 10f);
        }

        CurrectScaleOfBall = Mathf.Max(CurrectScaleOfBall - 0.05f, 1.3f);
        SpawnAndConfigureBall(CurrectScaleOfBall, mass: 3f, linearDamping: 0.5f);
    }

    public void LevelFive()
    {
        int score = gameManager != null ? gameManager.score : 0;
        if (score > 300 && score < 450)
        {
            BallForce = Mathf.Clamp(BallForce + 0.3f, 3f, 13f);
        }

        CurrectScaleOfBall = Mathf.Max(CurrectScaleOfBall - 0.01f, 1.2f);
        SpawnAndConfigureBall(CurrectScaleOfBall, mass: 3.2f, linearDamping: 0.6f);
    }

    public void LevelSix()
    {
        int score = gameManager != null ? gameManager.score : 0;
        if (score > 450 && score < 550)
        {
            BallForce = Mathf.Clamp(BallForce + 0.6f, 10f, 50f);
        }

        CurrectScaleOfBall = Mathf.Max(CurrectScaleOfBall - 0.01f, 1f);
        SpawnAndConfigureBall(CurrectScaleOfBall, mass: 3.2f, linearDamping: 0.6f);
    }

    public void LevelSeven()
    {
        int score = gameManager != null ? gameManager.score : 0;
        if (score > 550 && score < 700)
        {
            BallForce = Mathf.Clamp(BallForce + 0.6f, 10f, 55f);
        }

        CurrectScaleOfBall = Mathf.Max(CurrectScaleOfBall - 0.01f, 0.8f);
        SpawnAndConfigureBall(CurrectScaleOfBall, mass: 3.8f, linearDamping: 0.8f);
    }

    public void LevelEight()
    {
        int score = gameManager != null ? gameManager.score : 0;
        if (score > 700 && score < 1000)
        {
            BallForce = Mathf.Clamp(BallForce + 0.6f, 10f, 60f);
        }

        CurrectScaleOfBall = Mathf.Max(CurrectScaleOfBall - 0, 0.8f);
        SpawnAndConfigureBall(CurrectScaleOfBall, mass: 3.8f, linearDamping: 0.8f);
    }

    public void LevelNine()
    {
        int score = gameManager != null ? gameManager.score : 0;
        if (score > 1000 && score < 2000)
        {
            BallForce = Mathf.Clamp(BallForce + 0.8f, 10f, 65f);
        }

        CurrectScaleOfBall = Mathf.Max(CurrectScaleOfBall - 0.01f, 0.7f);
        SpawnAndConfigureBall(CurrectScaleOfBall, mass: 4.2f, linearDamping: 1f);
    }

    #endregion
}
