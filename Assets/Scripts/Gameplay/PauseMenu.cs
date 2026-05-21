using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{

    public AudioSource BGM;
    public AudioSource PauseSFX;
    public GameManager gameManager;
    Toggle ToggleButton;

    private void OnEnable()
    {
        VisualElement root =GetComponent<UIDocument>().rootVisualElement;
        Button RestartButton = root.Q<Button>("RestartButton");
        RestartButton.clicked += RestartGame;
        Button HomeButton = root.Q<Button>("HomeButton");
        HomeButton.clicked += GoToHome;

        //Coins update
       Label Coins =  root.Q<Label>("Coins");
        int currentcoinCount = gameManager.score;
        Coins.text = currentcoinCount.ToString();

        //Time update
        Label Time =root.Q<Label>("Time");
        float savedTime = PlayerPrefs.GetFloat("SurvivalTime", 0f);

        int minutes = Mathf.FloorToInt(savedTime / 60f);
        int seconds = Mathf.FloorToInt(savedTime % 60f);

        Time.text = minutes.ToString("00") + ":" + seconds.ToString("00") + "s";

        //Hearts update
        Label Hearts = root.Q<Label>("Hearts");
        int currentHealthCount = gameManager.Health;
        Hearts.text = currentHealthCount.ToString();

        
        
    }

    void GoToHome() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    
    }

    void RestartGame() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1.0f;
    }


    void OnApplicationFocus(bool hasFocus)
    {
        // Window/tab lost focus
        if (!hasFocus)
        {
            PauseGame();
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        // Mobile/browser tab pause
        if (pauseStatus)
        {
            PauseGame();
        }
    }


    public void PauseGame()
    {
             Time.timeScale = 0f;
             gameObject.SetActive(true);
             
        PauseSFX.PlayOneShot(PauseSFX.clip);
     GameObject[] Balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject ball in Balls) { 
          ball.GetComponent<CircleCollider2D>().enabled = false;
        }
    }

    public void ResumeGame()
    {
        GameObject[] Balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject ball in Balls)
        {
            ball.GetComponent<CircleCollider2D>().enabled = true;
        }
        if (gameObject != null)
            gameObject.SetActive(false);
        Time.timeScale = 1f;
        PauseSFX.PlayOneShot(PauseSFX.clip);
       
    }
}

