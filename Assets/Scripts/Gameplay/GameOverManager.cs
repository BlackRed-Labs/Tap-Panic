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

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        SurvivalTimeText = root.Q<Label>("SurvivalTimeText");
        CoinCount = root.Q<Label>("CoinCount");
        BestTime = root.Q<Label>("BestTime");
        root.Q<Button>("PlayAgain").clicked += PlayAgain;
        

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
        int coins = PlayerPrefs.GetInt("Score", 0);

        StartCoroutine(AnimateCoins(coins));
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

    #region Total Time
    private void SurvivalTime() {
        float savedTime = PlayerPrefs.GetFloat("SurvivalTime", 0f);

        int minutes = Mathf.FloorToInt(savedTime / 60f);
        int seconds = Mathf.FloorToInt(savedTime % 60f);

        SurvivalTimeText.text = minutes.ToString("00") + ":" + seconds.ToString("00") + "s";
    }
    #endregion
    
    #region Play again
    private void PlayAgain() { 
     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
     MissedTap.SetActive(true);
     Time.timeScale = 1f; 
    }
    #endregion

    #region mainmenu button
    private void MainMenuButton() { 
     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
    }
    #endregion
}
