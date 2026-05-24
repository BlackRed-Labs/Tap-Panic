using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameOverManager : MonoBehaviour
{
    private Label SurvivalTimeText;
    private Label CoinCount;
    public AudioSource CoincollectSFX;
    public GameObject MissedTap;
    private Label BestTime;
    public CoinManager CoinManager;
    

    //2X button variable
    Button _TwoXButton;
    int twoXButtonpressedCount = 0;

    //Score variable
    int coins;

    private void OnEnable()
    {
        //getting score 
        coins = PlayerPrefs.GetInt("Score", 0);

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        SurvivalTimeText = root.Q<Label>("SurvivalTimeText");
        CoinCount = root.Q<Label>("CoinCount");
        BestTime = root.Q<Label>("BestTime");

        //Play again button
        root.Q<Button>("PlayAgain").clicked += ShowIntestitialAds;

        //2X button
        _TwoXButton = root.Q<Button>("TwoXButton");
        _TwoXButton.clicked += CoindoubleAfterWatingAds;
        

        SurvivalTime();
        CoinsCount();
        Besttime();
    }

    private void Besttime() 
    {
       float bestTime = PlayerPrefs.GetFloat("BestTime", 0f);
       int minutes = Mathf.FloorToInt(bestTime / 60f);
       int seconds = Mathf.FloorToInt(bestTime % 60f);
       BestTime.text = minutes.ToString("00") + ":" + seconds.ToString("00") + "s";
    }

    #region coincount
    private void CoinsCount() { 
        
        StartCoroutine(AnimateCoins(coins));
        CoinManager.AddCoinsToTotal(coins);
    }

    IEnumerator AnimateCoins(int targetCoins)
    {
        int startCoins = 0;
        while (startCoins < targetCoins) {

            CoinCount.text = startCoins.ToString();
            startCoins++;
            CoincollectSFX.PlayOneShot(CoincollectSFX.clip);
            yield return null;
        }

    }
    #endregion

    #region survived Time
    private void SurvivalTime() {
        float savedTime = PlayerPrefs.GetFloat("SurvivalTime", 0f);

        int minutes = Mathf.FloorToInt(savedTime / 60f);
        int seconds = Mathf.FloorToInt(savedTime % 60f);

        SurvivalTimeText.text = minutes.ToString("00") + ":" + seconds.ToString("00") + "s";
    }
    #endregion

    #region Play again

    void ShowIntestitialAds() { 
      
        CrazyGamesAdsManager.Instance.ShowMidgameAd(PlayAgain);

    }
    private void PlayAgain() {
         // Show a midgame ad before restarting the level
        CrazyGamesManager.Instance.OnGameplayBegins(); // Notify CrazyGames SDK that gameplay has started again
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
     MissedTap.SetActive(true);
     Time.timeScale = 1f; 
    }
    #endregion

    #region 2X button

    void CoindoubleAfterWatingAds() {
        CrazyGamesAdsManager.Instance.ShowRewardedAd(TwoXbutton);
    }   

    private void TwoXbutton() 
    {
        twoXButtonpressedCount++;
        coins = coins*2;
        StartCoroutine(AnimateCoins(coins));
        CoinManager.AddCoinsToTotal(coins);
       
        if (twoXButtonpressedCount >= 2)
        {
            _TwoXButton.SetEnabled(false);
        }

    }


    #endregion

    #region mainmenu button
    private void MainMenuButton() { 
     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
    }
    #endregion
}
