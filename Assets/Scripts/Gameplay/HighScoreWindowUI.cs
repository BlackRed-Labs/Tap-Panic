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
    GameObject[] Balls;
    Coroutine currentCoinAnimationCoroutine; // Track active coroutine

    private void OnEnable()
    {
       

        AudioManager.Instance?.PauseBGM();

        Balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject ball in Balls)
        {
            if (ball != null)
            {
                ball.SetActive(false);
            }
        }

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        Label HighscoreText = root.Q<Label>("NewBestTime");

        //New Best
        float NewBest = PlayerPrefs.GetFloat("BestTime", 0f);
        StartCoroutine(CountAnimation(NewBest, HighscoreText));

        //Extra coins
        Earnedcoins = root.Q<Label>("EarnedCoins");
        newBest = Mathf.RoundToInt(NewBest) * 2;

        //2X button
        twoXbutton = root.Q<Button>("TwoXbutton");
        twoXbutton.clicked += TwoXButton;

        //Continue button
        root.Q<Button>("Continue").clicked += ContinueButton;

        //Load Total coins
        Label TotalCoinText = root.Q<Label>("TotalCoins");
        Totalconis = PlayerPrefs.GetInt("TotalCoins", 250);
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

        // Start coin animation after timer completes
        currentCoinAnimationCoroutine = StartCoroutine(CoincountAnimation(0, newBest));
    }

    //coin count animation - now accepts start and end values
    private IEnumerator CoincountAnimation(int startCoins, int TotalCoins)
    {
        int currentCoins = startCoins;

        while (currentCoins < TotalCoins)
        {
            currentCoins++;
            Earnedcoins.text = currentCoins.ToString();
            yield return null;
            coinCountSFX.PlayOneShot(coinCountSFX.clip);
        }
        
        // Ensure final value is set
        Earnedcoins.text = TotalCoins.ToString();
    }


    //2X button
    private void TwoXButton()
    {
        TwoXcount++;
        
        // Stop any currently running coin animation
        if (currentCoinAnimationCoroutine != null)
        {
            StopCoroutine(currentCoinAnimationCoroutine);
        }

        int previousBest = newBest;
        newBest = newBest * 2;
        
        // Start new animation from previous count to new count
        currentCoinAnimationCoroutine = StartCoroutine(CoincountAnimation(previousBest, newBest));

        if (TwoXcount >= 2)
        {
            twoXbutton.SetEnabled(false);
        }
    }

    //Continue button
    private void ContinueButton()
    {
        // Stop any running coroutine before continuing
        if (currentCoinAnimationCoroutine != null)
        {
            StopCoroutine(currentCoinAnimationCoroutine);
        }

        if (!twoXbutton.enabledSelf)
        {
            twoXbutton.SetEnabled(true);
        }

        CoinManager.AddCoinsToTotal(newBest);
        AudioManager.Instance?.UnpauseBGM();
        GameManager.Instance.GameOver();

    }
}