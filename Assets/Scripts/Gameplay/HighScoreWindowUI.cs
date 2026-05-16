using UnityEngine;
using UnityEngine.UIElements;

public class HighScoreWindowUI : MonoBehaviour
{
    Label HighscoreText;
    private void OnEnable()
    {
        VisualElement root =GetComponent<UIDocument>().rootVisualElement;
        HighscoreText = root.Q<Label>("NewBestTime");

    }


    public void UpdateNewBestTime() {
        float BestTime = PlayerPrefs.GetFloat("BestTime", 0f);
        float SurvivedTime = PlayerPrefs.GetFloat("SurvivalTime", 0f);
        if (SurvivedTime > BestTime)
        {
            PlayerPrefs.SetFloat("BestTime", SurvivedTime);
            HighscoreText.text = SurvivedTime.ToString();
        }

    }
}
