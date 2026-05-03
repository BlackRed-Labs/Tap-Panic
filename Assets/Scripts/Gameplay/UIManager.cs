using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{

    private Label scoreLabel;
    private Label SurvivedTime;
    private Image[] Hearts;
    private VisualElement HeartsParent;
    List<VisualElement> hearts = new List<VisualElement>();

    void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        scoreLabel = root.Q<Label>("ScoreText");
        SurvivedTime = root.Q<Label>("SurvivalTime");
        HeartsParent = root.Q<VisualElement>("Hearts");
        
        hearts.Add(HeartsParent.Q<VisualElement>("Heart1"));
        hearts.Add(HeartsParent.Q<VisualElement>("Heart2"));
        hearts.Add(HeartsParent.Q<VisualElement>("Heart3"));
        hearts.Add(HeartsParent.Q<VisualElement>("Heart4"));
        hearts.Add(HeartsParent.Q<VisualElement>("Heart5"));

    }

    public void RemoveLife(int Life)
    {
        hearts[Life].style.display = DisplayStyle.None;
    }

    public void AddLife(int Life)
    {
        hearts[Life].style.display = DisplayStyle.Flex;
    }

    public void AddScore(int score)
    {
        scoreLabel.text = score.ToString();
    }

    public void AddSurvivalTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        SurvivedTime.text = minutes.ToString("00") + ":" + seconds.ToString("00");
       
    }

}
