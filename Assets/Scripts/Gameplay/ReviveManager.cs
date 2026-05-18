using System;
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
    public AudioSource ReviveSFX;

    private void OnEnable()
    {
       VisualElement root  = GetComponent<UIDocument>().rootVisualElement;
        root.Q<Button>("CloseButton").clicked+=Closebutton;
        Button reviveButton = root.Q<Button>("Revivebutton");
        reviveButton.clicked+= ReviveButton;
       GroupBox priceBox = root.Q<GroupBox>("PriceGroup");


        PriceText = root.Q<Label>("Price");
        PriceText.text = RevivePrice.ToString();
        
        // Save high score
        SurvivedTime = GameManager.Instance.survivalTime;
        PlayerPrefs.SetFloat("SurvivalTime", SurvivedTime);
    
        SurvivedTime = PlayerPrefs.GetFloat("SurvivalTime", 0f);
        BestTime = PlayerPrefs.GetFloat("BestTime", 0f);

        if (SurvivedTime >= BestTime) {
            BestTime = SurvivedTime;
            PlayerPrefs.SetFloat("BestTime", BestTime);
        }

        //update Best Time
      
        Label BestTimeText = root.Q<Label>("BestTime");
        int minutes = Mathf.FloorToInt(BestTime / 60);
        int seconds = Mathf.FloorToInt(BestTime % 60);
        BestTimeText.text = minutes.ToString("00") + ":" + seconds.ToString("00") + "s";


        //update the remaing coin count
        Label remainingCOins = root.Q<Label>("RemainingCoins");
        totalCoins = PlayerPrefs.GetInt("TotalCoins", 500);
        remainingCOins.text = totalCoins.ToString();
        
        if (totalCoins < RevivePrice)
        {
            reviveButton.SetEnabled(false);
            priceBox.SetEnabled(false);
        }


        //play Revive SFX
        BGM.Pause();
        ReviveSFX.Play();
    }

    private void Closebutton()
    {
        // Best Time Logic
        
        gameObject.SetActive(false);
        BGM.Play();
        ReviveSFX.Stop();
      
        if (SurvivedTime >= BestTime)
        {
            
            if (NewBestWindow != null)
                NewBestWindow.SetActive(true);
        }
        else
        {
            GameManager.GameOver();
        }
        
    }

    private void ReviveButton() {
        
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


}
