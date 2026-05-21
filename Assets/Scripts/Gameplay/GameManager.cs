using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
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
    public float survivalTime;
    bool isAlive = true;
    private CinemachineImpulseSource impulseSource;
    public CinemachineCamera vcam;
    public GameObject MissedTap;
    public ReviveManager reviveManager;
    public GameObject Bounceffect;
    public GameObject DestroyEffect;
    public GameObject MouseclickEffectPrefab;
    public GameObject HowToPlayWindow;
    public GameObject CountDownText;
    public AudioSource ScreenshakeSFX;
    public GameObject BestTime;
    public PauseMenu pauseMenu;
    bool isPauseMenuActive = false;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 200;
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void Start()
    {

        if (PlayerPrefs.GetString("IsHowToPlayShown", "No") == "No")
        {
            if (HowToPlayWindow != null)
            {
                HowToPlayWindow.SetActive(true);
            }

            PlayerPrefs.SetString("IsHowToPlayShown", "Yes");
            PlayerPrefs.Save();
        }
        else
        {
            CountDownText.SetActive(true);
            CountdownTimer.Instance.UpdateCountDown(3);
        }
    }
   
    #region Start Game
     
    public void StartGame()
    {

        BGM.Play();
        MissedTap.SetActive(true);
        difficultyManager.LevelOne();
        difficultyManager.LevelOne();
    }
    #endregion

    #region Survival time , Mouse input effect , Pause Menu  activate/deactivate

    [HideInInspector]
    public bool isGameStarted = false;

    private void Update()
    {
        #region Pause menu Active/disable
        // ESC key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPauseMenuActive && Time.timeScale>0)
            {
                pauseMenu.PauseGame();
                isPauseMenuActive=true;
                MissedTap.SetActive(false);
            }

            else if (isPauseMenuActive && Time.timeScale<1) { 
            
              pauseMenu.ResumeGame();
                isPauseMenuActive=false;
                MissedTap.SetActive(true);
            }
            
           
        }
        #endregion

        #region Survive Time update
        if (!isAlive) return;

        survivalTime += Time.deltaTime;

        if(isGameStarted == true)
        {
            //formatted internally
            UIManager.AddSurvivalTime(survivalTime);
            
        }
        #endregion

        #region Mouse click effect
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.timeScale > 0)
            {
                SpawnEffect(Input.mousePosition);
            }
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (Time.timeScale > 0)
            {
                SpawnEffect(Input.GetTouch(0).position);
            }
        }
    }
    
    void SpawnEffect(Vector3 screenPos)
    {
    
        screenPos.z = Mathf.Abs(Camera.main.transform.position.z);

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;

        GameObject effect = Instantiate(MouseclickEffectPrefab, worldPos, Quaternion.identity);
        #endregion
    }

    #endregion

    #region Spawn ball

    public GameObject SpawnBall()
    {
        Spawnedball = Instantiate(ballPrefab, new Vector2(UnityEngine.Random.Range(-1.3f, 1.3f), UnityEngine.Random.Range(0f, 2f)), Quaternion.identity);
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
        UIManager.RemoveLife(4-Health);
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

    #region camera shake
    public void cameraShake() 
    { 
      impulseSource.GenerateImpulse();
      BGM.Pause();
        ScreenshakeSFX.PlayOneShot(ScreenshakeSFX.clip);
        BGM.UnPause();
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

   /* #region Ball Bounce Effect

    public void BallBounceEffect(Color Ballcolor, Vector2 hitPoint)
    {
        GameObject bounceEffect = Instantiate(Bounceffect, hitPoint, Quaternion.identity);
        var main = bounceEffect.GetComponent<ParticleSystem>().main;
        main.startColor = Ballcolor;
        bounceEffect.GetComponent<ParticleSystem>().Play();
    }
    #endregion*/


}


