using UnityEngine;
using UnityEngine.UIElements;

public class HowToPlayWindow : MonoBehaviour
{
    public DifficultyManager difficultyManager;
    public GameObject MissedTap;
    public GameManager gameManager;

    private void OnEnable()
    {
       VisualElement Root =GetComponent<UIDocument>().rootVisualElement;
        Root.Q<Button>("GotIt").clicked += GotItButton;
        
    }

    public void GotItButton() { 
        gameObject.SetActive(false);
        gameManager.CountDownText.SetActive(true);
        CountdownTimer.Instance.UpdateCountDown(3);

    }
}
