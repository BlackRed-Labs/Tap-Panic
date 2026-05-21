using System.Collections.Generic;
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

    private void OnEnable()
    {
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

        // Pause BGM
        BGM?.Pause();
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

        var priceBox = root.Q<GroupBox>("PriceGroup");

        // Wire up button callbacks
        closeButton?.RegisterCallback<ClickEvent>(_ => OnCloseButton());
        reviveButton?.RegisterCallback<ClickEvent>(_ => OnReviveButton());
        watchAdButton?.RegisterCallback<ClickEvent>(_ => OnWatchAdButton());

        // Set price display
        if (priceText != null) priceText.text = RevivePrice.ToString();

        // Disable revive if insufficient coins
        if (reviveButton != null) reviveButton.SetEnabled(totalCoins >= RevivePrice);
        if (priceBox != null) priceBox.SetEnabled(totalCoins >= RevivePrice);
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
        BGM?.UnPause();

        CoinManager?.RemoveCoinsFromTotal(RevivePrice);
        RevivePrice *= 2;

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

    private void OnWatchAdButton()
    {
        // Watch ad and revive (same as revive button for now)
        OnReviveButton();
    }

    private void OnDisable()
    {
        // Re-enable ball colliders when window closes
        if (activeBalls != null && activeBalls.Length > 0)
        {
            EnableBallColliders(activeBalls);
        }
    }
}