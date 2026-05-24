using System.Collections;
using Unity.Cinemachine;
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
    public GameObject DestroyEffect;
    public GameObject HowToPlayWindow;
    public GameObject CountDownText;
    public AudioSource ScreenshakeSFX;
    public GameObject BestTime;
    public PauseMenu pauseMenu;
   
    [HideInInspector] 
    public bool isPauseMenuActive = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        
        Instance = this;
        Application.targetFrameRate = 200;
        impulseSource = GetComponent<CinemachineImpulseSource>();
        
    }

    void Start()
    {
        CrazyGamesManager.Instance.OnGameplayBegins(); // Notify CrazyGames SDK that gameplay has started
       

        if (PlayerPrefs.GetString("IsHowToPlayShown", "No") == "No")
        {
            if (HowToPlayWindow != null)
            {
                HowToPlayWindow.SetActive(true);
                Time.timeScale = 0f;
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

    [HideInInspector]
    public bool isGameStarted = false;

    private void Update()
    {
        #region Pause menu Active/disable
        // ESC key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPauseMenuActive && Time.timeScale > 0)
            {
                pauseMenu.PauseGame();
            }

            else if (isPauseMenuActive && Time.timeScale < 1)
            {
                pauseMenu.ResumeGame();
            }


        }
        #endregion

        #region Survive Time update
        if (!isAlive) return;

        survivalTime += Time.deltaTime;

        if (isGameStarted == true)
        {
            //formatted internally
            UIManager.AddSurvivalTime(survivalTime);

        }
        #endregion
    }

    #region Spawn ball

    public GameObject SpawnBall()
    {
        Spawnedball = Instantiate(ballPrefab, new Vector2(UnityEngine.Random.Range(-0.16f, 2.5f), UnityEngine.Random.Range(0f, 1.8f)), Quaternion.identity);
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
        if (Health <= 0)
        {
            Health = 0;
        }
        cameraShake();
        UIManager.RemoveLife(4 - Health);
    }
    #endregion

    #region Game Over
    public void GameOver()
    {
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject ball in balls)
        {
            Destroy(ball);
        }

        difficultyManager.StopAllCoroutines();
        reviveManager.RevivePrice = 50;  
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
    public void destroyEffect(Color Ballcolor, Vector3 BallLastPos)
    {
        GameObject destroyEffect = Instantiate(DestroyEffect, BallLastPos, Quaternion.identity);
        var main = destroyEffect.GetComponent<ParticleSystem>().main;
        main.startColor = Ballcolor;
        destroyEffect.GetComponent<ParticleSystem>().Play();
    }

    #endregion

   


}


