using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class HighScoreWindowUI : MonoBehaviour
{
    public AudioSource CountSFX;
    Label Earnedcoins;
    public AudioSource coinCountSFX;
    int newBest;
    int Totalconis;
    public CoinManager CoinManager;

    int TwoXcount = 0;
    Button twoXbutton;


    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        Label HighscoreText = root.Q<Label>("NewBestTime");


        //New Best
        float NewBest = PlayerPrefs.GetFloat("BestTime", 0f);
        StartCoroutine(CountAnimation(NewBest, HighscoreText));

        //Extra coins
        Earnedcoins = root.Q<Label>("EarnedCoins");
        newBest = Mathf.RoundToInt(NewBest)*2;

        //2X button
        twoXbutton = root.Q<Button>("TwoXbutton");
        twoXbutton.clicked += TwoXButton;

        //Continue button
        root.Q<Button>("Continue").clicked += ContinueButton;

        //Load Total coins
        Label TotalCoinText = root.Q<Label>("TotalCoins");
        Totalconis = PlayerPrefs.GetInt("TotalCoins", 500);
        TotalCoinText.text = Totalconis.ToString();


    }

    //timer count animation
   private IEnumerator CountAnimation(float targetTime, Label text)
    {
        float currentTime = 0f;

        while (currentTime < targetTime)
        {
            currentTime += Time.unscaledDeltaTime * 20f;

            if (currentTime > targetTime)
                currentTime = targetTime;

            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);

            text.text = minutes.ToString("00") + ":" + seconds.ToString("00") + "s";

            yield return null;

            CountSFX.PlayOneShot(CountSFX.clip);
            
        }
       
        StartCoroutine(CoincountAnimation(newBest));

    }

    //coin count animation
   private IEnumerator CoincountAnimation(int TotalCoins) {

        int currentCoins = 0;

        while (currentCoins < TotalCoins)
        {
            currentCoins++;
            Earnedcoins.text = currentCoins.ToString();
            yield return null;
            coinCountSFX.PlayOneShot(coinCountSFX.clip);

        }
        
       

    }

    //2X button
    private void TwoXButton() 
    {
        TwoXcount++;
        newBest = newBest * 2;
        StartCoroutine(CoincountAnimation(newBest));

        if (TwoXcount >= 2) 
        { 
          twoXbutton.SetEnabled(false);
        
        }

    }

    //Continue button
    private void ContinueButton() 
    {
        if (!twoXbutton.enabledSelf) { 
          twoXbutton.SetEnabled(true);
        }

        CoinManager.AddCoinsToTotal(newBest);
        GameManager.Instance.GameOver();
    
    }


}
