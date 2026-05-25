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
        root.Q<Button>("PlayAgain").clicked += PlayAgain;

        //2X button
        _TwoXButton = root.Q<Button>("TwoXButton");
        _TwoXButton.clicked += TwoXbutton;



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
        
        StartCoroutine(AnimateCoins(0, coins));
        CoinManager.AddCoinsToTotal(coins);
    }

    /// <summary>
    /// Animates coin count from startCoins to targetCoins
    /// </summary>
    IEnumerator AnimateCoins(int startCoins, int targetCoins)
    {
        int currentCoins = startCoins;
        while (currentCoins < targetCoins) 
        {
            CoinCount.text = currentCoins.ToString();
            currentCoins++;
            CoincollectSFX.PlayOneShot(CoincollectSFX.clip);
            yield return null;
        }
        // Ensure final value is displayed
        CoinCount.text = targetCoins.ToString();
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

  
    private void PlayAgain() {
         // Show a midgame ad before restarting the level
       
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
     MissedTap.SetActive(true);
     Time.timeScale = 1f; 
    }
    #endregion

    #region 2X button

     

    private void TwoXbutton() 
    {
        twoXButtonpressedCount++;
        int previousCoins = coins;
        coins = coins * 2;
        
        // Animate from previous coin count to new count
        StartCoroutine(AnimateCoins(previousCoins, coins));
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
