using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class ReviveManager : MonoBehaviour
{
    [Header("References")]
    public GameManager GameManager;
    public UIManager UIManager;
    public AudioSource BGM;
    public CoinManager CoinManager;
    public CountdownTimer countdownTimer;
    public GameObject NewBestWindow;

    [Header("Settings")]
    [HideInInspector] public int RevivePrice = 50;

    // Cached UI elements
    private Label priceText;
    private Label bestTimeText;
    private Label remainingCoinsLabel;
    private Button reviveButton;
    private Button watchAdButton;
    private Button closeButton;

    // State
    private float survivedTime;
    private float bestTime;
    private int totalCoins;
    private GameObject[] activeBalls;
    int AdwatchedCount = 0;


    private void OnEnable()
    {
        CrazyGamesManager.Instance.OnGamePaused(); // Notify CrazyGames SDK that gameplay is paused (revive window open)
        // Cache balls early to avoid repeated FindGameObjectsWithTag calls
        activeBalls = GameObject.FindGameObjectsWithTag("Ball");
        DisableBallColliders(activeBalls);

        // Get UI root once
        var root = GetComponent<UIDocument>()?.rootVisualElement;
        if (root == null) return;

        // Cache all UI elements at once
        CacheUIElements(root);

        // Load and display survival/best time
        UpdateTimeDisplay();

        // Update coin display
        UpdateCoinDisplay();

        //update revive price display
        if (priceText != null) priceText.text = RevivePrice.ToString();

        // Disable revive if insufficient coins
        var priceBox = root.Q<GroupBox>("PriceGroup");

        if (reviveButton != null) reviveButton.SetEnabled(totalCoins >= RevivePrice);
        if (priceBox != null) priceBox.SetEnabled(totalCoins >= RevivePrice);

        // Pause BGM
        BGM?.Pause();


        //disable watch ad button after 3 watches
        if (AdwatchedCount >= 2 && watchAdButton != null)
        {
            watchAdButton.SetEnabled(false);
        }

        // Start coroutine to enable buttons after delay (prevents accidental clicks)
        StartCoroutine(ButtonEnabledelay(new Button[] { reviveButton, watchAdButton, closeButton }));

       

    }

    private void CacheUIElements(VisualElement root)
    {
        // Cache all UI elements for reuse
        closeButton = root.Q<Button>("CloseButton");
        reviveButton = root.Q<Button>("Revivebutton");
        watchAdButton = root.Q<Button>("WatchAdButton");
        priceText = root.Q<Label>("Price");
        bestTimeText = root.Q<Label>("BestTime");
        remainingCoinsLabel = root.Q<Label>("RemainingCoins");



        // Wire up button callbacks
        closeButton.clicked += OnCloseButton;
        reviveButton.clicked += OnReviveButton;
        watchAdButton.clicked += OnWatchAdButton;

    }

    private void UpdateTimeDisplay()
    {
        // Read survival time from GameManager (in-memory), not PlayerPrefs
        survivedTime = GameManager != null ? GameManager.survivalTime : 0f;
        bestTime = PlayerPrefs.GetFloat("BestTime", 0f);

        // Update best time if this run was better
        if (survivedTime > bestTime)
        {
            bestTime = survivedTime;
            PlayerPrefs.SetFloat("BestTime", bestTime);
        }

        // Format and display best time
        if (bestTimeText != null)
        {
            int minutes = Mathf.FloorToInt(bestTime / 60f);
            int seconds = Mathf.FloorToInt(bestTime % 60f);
            bestTimeText.text = minutes.ToString("00") + ":" + seconds.ToString("00") + "s";
        }
    }

    private void UpdateCoinDisplay()
    {
        totalCoins = PlayerPrefs.GetInt("TotalCoins", 500);
        if (remainingCoinsLabel != null) remainingCoinsLabel.text = totalCoins.ToString();
    }

    private void DisableBallColliders(GameObject[] balls)
    {
        foreach (var ball in balls)
        {
            if (ball != null)
            {
                var collider = ball.GetComponent<CircleCollider2D>();
                if (collider != null) collider.enabled = false;
            }
        }
    }

    private void EnableBallColliders(GameObject[] balls)
    {
        foreach (var ball in balls)
        {
            if (ball != null)
            {
                var collider = ball.GetComponent<CircleCollider2D>();
                if (collider != null) collider.enabled = true;
            }
        }
    }

    private void OnCloseButton()
    {
      
        gameObject.SetActive(false);
        BGM?.UnPause();

        // Show new best window or game over
        if (survivedTime >= bestTime)
        {
            NewBestWindow?.SetActive(true);
        }
        else
        {
            GameManager?.GameOver();
        }
    }

    private void OnReviveButton()
    {
        CrazyGamesManager.Instance.OnGameplayBegins(); // Notify CrazyGames SDK that gameplay has resumed (player revived)
        BGM?.UnPause();

        CoinManager?.RemoveCoinsFromTotal(RevivePrice);

        if (reviveButton != null && reviveButton.enabledSelf)
        {
            RevivePrice *= 2;
        }
        // Reset health and UI
        if (GameManager != null)
        {
            GameManager.Health = 5;
        }

        for (int i = 0; i < 5; i++)
        {
            UIManager?.AddLife(i);
        }

        // Resume game
        gameObject.SetActive(false);
        Time.timeScale = 1f;

        if (BGM != null) BGM.pitch = 1f;
        GameManager?.MissedTap?.SetActive(true);
    }

    IEnumerator ButtonEnabledelay(Button[] buttons)
    {
        bool[] wasEnabled = new bool[buttons.Length];

        // Temporarily disable only currently enabled buttons
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue;

            wasEnabled[i] = buttons[i].enabledSelf;

            if (wasEnabled[i])
            {
                buttons[i].SetEnabled(false);
            }
        }

        yield return new WaitForSecondsRealtime(0.08f);

        // Re-enable only buttons that were originally enabled
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue;

            if (wasEnabled[i])
            {
                buttons[i].SetEnabled(true);
            }
        }
    }


    private void OnWatchAdButton()
    {
        AdwatchedCount++;
        Debug.Log("Ad watched count: " + AdwatchedCount);
        CrazyGamesAdsManager.Instance.ShowRewardedAd(OnReviveButton);

    }

    private void OnDisable()
    {
        // Remove UI Toolkit callbacks
        if (closeButton != null)
            closeButton.clicked -= OnCloseButton;

        if (reviveButton != null)
            reviveButton.clicked -= OnReviveButton;

        if (watchAdButton != null)
            watchAdButton.clicked -= OnWatchAdButton;

        // Re-enable ball colliders
        if (activeBalls != null && activeBalls.Length > 0)
        {
            EnableBallColliders(activeBalls);
        }
    }

}
    

    
