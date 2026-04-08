using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject ballPrefab;
    GameObject Spawnedball;
    public float Ballforce;
    public int score = 0;
    public UIManager UIManager;
    public DifficultyManager difficultyManager;
    [HideInInspector]
    public int Health = 5;
    int BonusScore = 0;
    public int ScoreMultiplyerDelay;
    [HideInInspector]
    public bool isBonusScoreActive = false;
    public GameObject CoinPreFab;
    public Vector3 CoinCollectPos;
    public AudioSource CoinCollectSFX;
    [HideInInspector]
    public Vector2 BallLastPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        QualitySettings.vSyncCount = 0; // disable VSync
        Application.targetFrameRate = 100;

    }

    void Start()
    {

        difficultyManager.LevelOne();
    }

    #region Spawn ball

    public GameObject SpawnBall()
    {
        Spawnedball = Instantiate(ballPrefab, new Vector2(Random.Range(-1.3f, 1.3f), Random.Range(0f, 3f)), Quaternion.identity);
        return Spawnedball;
        
    }
    #endregion

    #region Score Syytem
    public void ScoreSystem()
    {
        score++;
        score = score + BonusScore;
        UIManager.AddScore(score);

    }

    #region double Score powerup

    public void DoubleScore() 
    {
        StartCoroutine(DoubpleScoreManager(ScoreMultiplyerDelay));

    }

    IEnumerator DoubpleScoreManager(int ScoreMultiplyerDelay)
    {
        isBonusScoreActive = true;
        BonusScore = 1;
        yield return new WaitForSeconds(ScoreMultiplyerDelay);
        BonusScore = 0;
        isBonusScoreActive = false;

    }

    #endregion

    #endregion

    #region Health Sysstem

    public void HealthSystem()
    {
        if (Health <= 0) return;
        Health--;
        UIManager.RemoveLife(Health);
    }
    #endregion

   
    #region Coin Reward System

    public void CoinReward()
    {

        int coinCount = 1;

        if (isBonusScoreActive) 
        {
            coinCount = 2;
        }

        if (score > 101 && score < 300)
        {
            coinCount = 2;
        }

        else if(score>300 && score <550)
        {
            coinCount = 3;
        }

        else if (score > 550 && score < 1000)
        {
            coinCount = 4;
        }

        else if (score > 1000 && score < 2000)
        {
            coinCount = 5;
        }



        StartCoroutine(SpawnCoinsOneByOne(coinCount));
        
    }

    IEnumerator SpawnCoinsOneByOne(int count)
    {
        Ball ball = FindAnyObjectByType<Ball>();

        for (int i = 0; i < count; i++)
        {
            GameObject coin = Instantiate(CoinPreFab, BallLastPos, Quaternion.identity);
            StartCoroutine(MoveCoin(coin));

            // delay between each coin spawn
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator MoveCoin(GameObject coin)
    {
        float speed = 12f;
        float popDuration = 0.25f;
        float maxPopScale = 3.5f;

        Vector3 originalScale = Vector3.zero;
        Vector3 overshootScale = Vector3.one * maxPopScale;
        Vector3 finalScale = Vector3.one;

        float t = 0f;

        // POP IN
        while (t < popDuration / 2)
        {
            t += Time.deltaTime;
            coin.transform.localScale = Vector3.Lerp(originalScale, overshootScale, t / (popDuration / 2));
            yield return null;
        }

        t = 0f;
        while (t < popDuration / 2)
        {
            t += Time.deltaTime;
            coin.transform.localScale = Vector3.Lerp(overshootScale, finalScale, t / (popDuration / 2));
            yield return null;
        }

        // MOVE TO COLLECT
        while (Vector3.Distance(coin.transform.position, CoinCollectPos) > 0.05f)
        {
            coin.transform.position = Vector3.MoveTowards(
                coin.transform.position,
                CoinCollectPos,
                speed * Time.deltaTime
            );
            yield return null;
        }

        CoinCollectSFX.Play();
        ScoreSystem();
        Destroy(coin);
    }

    #endregion



}
