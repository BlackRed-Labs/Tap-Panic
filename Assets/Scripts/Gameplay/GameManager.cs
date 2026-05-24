using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance { get; private set; } = null!;
    #endregion

    #region Ball Spawning
    public GameObject ballPrefab;
    GameObject Spawnedball;
    public float Ballforce;
    #endregion

    #region Scoring System
    public int score = 0;
    public int ScoreMultiplyerDelay;
    #endregion

    #region Health System
    public int Health = 5;
    #endregion

    #region UI References
    public UIManager UIManager;
    public GameObject GameoverWindow;
    public GameObject ReviveWindow;
    public GameObject HowToPlayWindow;
    public GameObject CountDownText;
    public GameObject BestTime;
    public PauseMenu pauseMenu;
    #endregion

    #region Audio
    [SerializeField]
    private AudioSource BGM;
    public AudioSource ScreenshakeSFX;
    #endregion

    #region Gameplay State
    public float survivalTime;
    bool isAlive = true;
    [HideInInspector]
    public bool isGameStarted = false;
    [HideInInspector]
    public bool isPauseMenuActive = false;
    #endregion

    #region Managers & Components
    public DifficultyManager difficultyManager;
    private CinemachineImpulseSource impulseSource;
    public CinemachineCamera vcam;
    public ReviveManager reviveManager;
    #endregion

    #region Visual Effects
    public GameObject MissedTap;
    public GameObject DestroyEffect;
    #endregion

    [Header("PreFabs")]
    [SerializeField]
    private GameObject HeartIcon;
    [SerializeField]
    private Vector3 HeartMovementTargetPosition;
    

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
        
        //Spawn 2 balls at the start of the game
        for (int i = 0; i < 2; i++) { 
            difficultyManager.LevelOne();
        }
    }
    #endregion

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
       
        UIManager.AddScore(score);
        PlayerPrefs.SetInt("Score", score);

    }

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

        difficultyManager.StopAllCoroutines();
        difficultyManager.ResetMilestoneFlags();
        reviveManager.RevivePrice = 100;  
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

    #region Health collect effect
    public void AddHealthEffect(Vector3 HeartSpawnPosition) { 
     
         StartCoroutine(HearticonMovememnt(HeartSpawnPosition));

    }

    IEnumerator HearticonMovememnt(Vector3 HeartSpawnPosition)
    {
        GameObject heart = Instantiate(HeartIcon, HeartSpawnPosition, Quaternion.identity);

        Vector3 targetposition = HeartMovementTargetPosition;

        float elapsedTime = 0f;

        float popInDuration = 0.25f;
        float moveDuration = 1f;
        float popOutDuration = 0.3f;

        Vector3 maxScale = new Vector3(0.3f, 0.3f, 0.3f);

        // Start invisible
        heart.transform.localScale = Vector3.zero;

        // ---------------- POP IN ----------------
        while (elapsedTime < popInDuration)
        {
            float t = elapsedTime / popInDuration;

            // Smooth easing-
            t = Mathf.SmoothStep(0f, 1f, t);

            heart.transform.localScale = Vector3.Lerp(Vector3.zero, maxScale, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        heart.transform.localScale = maxScale;

        // ---------------- MOVE ----------------
        elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration;

            // Smooth movement
            t = Mathf.SmoothStep(0f, 1f, t);

            heart.transform.position = Vector3.Lerp(HeartSpawnPosition, targetposition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        heart.transform.position = targetposition;

        // ---------------- POP OUT ----------------
        elapsedTime = 0f;

        while (elapsedTime < popOutDuration)
        {
            float t = elapsedTime / popOutDuration;

            t = Mathf.SmoothStep(0f, 1f, t);

            heart.transform.localScale = Vector3.Lerp(maxScale, Vector3.zero, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(heart);
    }
    #endregion
}













