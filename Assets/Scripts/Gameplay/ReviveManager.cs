using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class ReviveManager : MonoBehaviour
{
    public GameManager GameManager;
    public UIManager UIManager;
    public AudioSource BGM;

    [HideInInspector]
    public int RevivePrice = 50;

    Label PriceText;

    public GameObject NewBestWindow;

    float SurvivedTime;
    float BestTime;

    int totalCoins;

    public CoinManager CoinManager;
    public CountdownTimer countdownTimer;
    GameObject[] Balls;

    private void OnEnable()
    {
         Balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject ball in Balls)
        {
            if (ball != null)
            {
                ball.GetComponent<CircleCollider2D>().enabled = false;
            }
        }

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        // Close window Button
        root.Q<Button>("CloseButton").clicked += Closebutton;
          
        //Revive Button
        Button reviveButton = root.Q<Button>("Revivebutton");
        reviveButton.clicked += ReviveButton;

        //WatchAds Button
        Button _WatchAdButton = root.Q<Button>("WatchAdButton");
        _WatchAdButton.clicked += WatchAdButton;
        


        GroupBox priceBox = root.Q<GroupBox>("PriceGroup");

        // Price
        PriceText = root.Q<Label>("Price");
        PriceText.text = RevivePrice.ToString();

        // Save Survival Time
        SurvivedTime = GameManager.Instance.survivalTime;
        PlayerPrefs.SetFloat("SurvivalTime", SurvivedTime);

        SurvivedTime = PlayerPrefs.GetFloat("SurvivalTime", 0f);
        BestTime = PlayerPrefs.GetFloat("BestTime", 0f);

        if (SurvivedTime >= BestTime)
        {
            BestTime = SurvivedTime;
            PlayerPrefs.SetFloat("BestTime", BestTime);
        }

        // Best Time UI
        Label BestTimeText = root.Q<Label>("BestTime");

        int minutes = Mathf.FloorToInt(BestTime / 60f);
        int seconds = Mathf.FloorToInt(BestTime % 60f);

        BestTimeText.text = minutes.ToString("00") + ":" + seconds.ToString("00") + "s";

        // Remaining Coins
        Label remainingCoins = root.Q<Label>("RemainingCoins");

        totalCoins = PlayerPrefs.GetInt("TotalCoins", 500);

        remainingCoins.text = totalCoins.ToString();

        // Disable revive if not enough coins
        if (totalCoins < RevivePrice)
        {
            reviveButton.SetEnabled(false);
            priceBox.SetEnabled(false);
        }

        // Audio
        BGM.Pause();   
      
    }


    private void Closebutton()
    {
        

        gameObject.SetActive(false);

        BGM.UnPause();

        if (SurvivedTime >= BestTime)
        {
            if (NewBestWindow != null)
            {
                NewBestWindow.SetActive(true);
            }
        }
        else
        {
            GameManager.GameOver();
        }
    }

    private void ReviveButton()
    {
        
        BGM.UnPause();

        CoinManager.RemoveCoinsFromTotal(RevivePrice);

        RevivePrice *= 2;

        GameManager.Health = 5;

        for (int i = 0; i < 5; i++)
        {
            UIManager.AddLife(i);
        }

        gameObject.SetActive(false);

        Time.timeScale = 1f;

        BGM.pitch = 1f;

        GameManager.MissedTap.SetActive(true);
    }

    void WatchAdButton() { 
    
     ReviveButton();
    
    }

    private void OnDisable()
    {
        Balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject ball in Balls)
        {
            if (ball != null)
            {
                ball.GetComponent<CircleCollider2D>().enabled = true;
            }
        }

    }
}