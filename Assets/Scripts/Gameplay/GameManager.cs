using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject ballPrefab;
    GameObject Spawnedball;
    public float Ballforce;
    public int score = 0;
    public UIManager UIManager;
    public DifficultyManager difficultyManager;
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
    [SerializeField]
    private GameObject GameoverWindow;
    [SerializeField]
    private GameObject ReviveWindow;
    [SerializeField]
    private AudioSource BGM;
    float survivalTime = 0f;
    bool isAlive = true;
    private CinemachineImpulseSource impulseSource;
    public CinemachineCamera vcam;
    public GameObject MissedTap;
    public ReviveManager reviveManager;
    public GameObject Bounceffect;
    public GameObject DestroyEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        QualitySettings.vSyncCount = 0; // disable VSync
        Application.targetFrameRate = 100;
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void Start()
    {
        
        difficultyManager.LevelOne();
        difficultyManager.LevelOne();

    }

    #region Spawn ball

    public GameObject SpawnBall()
    {
        Spawnedball = Instantiate(ballPrefab, new Vector2(UnityEngine.Random.Range(-1.3f, 1.3f), UnityEngine.Random.Range(0f, 3f)), Quaternion.identity);
        return Spawnedball;

    }
    #endregion

    #region Score Syytem
    public void ScoreSystem()
    {
        score++;
        score = score + BonusScore;
        UIManager.AddScore(score);
        PlayerPrefs.SetInt("Score", score);

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
        Health--;
        cameraShake();
        UIManager.RemoveLife(Health);
    }
    #endregion

    #region Game Over
    public void GameOver()
    {
        
        difficultyManager.StopAllCoroutines();
        UIManager.enabled = false;
        reviveManager.RevivePrice = 50;
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject ball in balls)
        {
            Destroy(ball);
        }
        MissedTap.SetActive(false);
        Time.timeScale = 0f;
         PlayerPrefs.SetFloat("SurvivalTime", survivalTime);

        //Best Time Logic
        float SurvivedTime = PlayerPrefs.GetFloat("SurvivalTime", 0f);
        float BestTime = PlayerPrefs.GetFloat("BestTime", 0f);

        if (SurvivedTime > BestTime)
        {
            PlayerPrefs.SetFloat("BestTime", SurvivedTime);
        }
        GameoverWindow.SetActive(true);
        BGM.pitch = 0.5f;
    }
    #endregion

    #region Revive 
    public void Revive() 
    {
        
        difficultyManager.StopAllCoroutines();
        MissedTap.SetActive(false);
        Time.timeScale = 0f;
        PlayerPrefs.SetFloat("SurvivalTime", survivalTime);

        //Best Time Logic
        float SurvivedTime = PlayerPrefs.GetFloat("SurvivalTime", 0f);
        float BestTime = PlayerPrefs.GetFloat("BestTime", 0f);

        if (SurvivedTime > BestTime)
        {
            PlayerPrefs.SetFloat("BestTime", SurvivedTime);
        }
        ReviveWindow.SetActive(true);
        BGM.pitch = 0.5f;
    }
    #endregion

    #region Survivl Timer

    void Update()
    {
        if (!isAlive) return;

        survivalTime += Time.deltaTime;

        //formatted internally
        UIManager.AddSurvivalTime(survivalTime);

        
    }
    #endregion

    #region camera shake
    public void cameraShake() 
    { 
      impulseSource.GenerateImpulse();
    }
    #endregion
    #region Ball destroy effect
    public void destroyEffect(Color Ballcolor) { 
       GameObject destroyEffect = Instantiate(DestroyEffect, BallLastPos, Quaternion.identity);
        var main = destroyEffect.GetComponent<ParticleSystem>().main;
        main.startColor = Ballcolor;
        destroyEffect.GetComponent<ParticleSystem>().Play();
    }

    #endregion

}


