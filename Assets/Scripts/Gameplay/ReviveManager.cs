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

    private void OnEnable()
    {
       VisualElement root  =GetComponent<UIDocument>().rootVisualElement;
        root.Q<Button>("CloseButton").clicked+=Closebutton;
        root.Q<Button>("ReviveButton").clicked += ReviveButton;
        PriceText = root.Q<Label>("Price");
        PriceText.text = RevivePrice.ToString();
    }

    private void Closebutton() 
    {
        gameObject.SetActive(false);
        GameManager.GameOver();
    }

    private void ReviveButton() { 
        gameObject.SetActive(false);
        RevivePrice *= 2; 
        GameManager.Health = 5;
        for (int i = 0; i < 5; i++)
        {
            UIManager.AddLife(i);
        }
        Time.timeScale = 1f;
        BGM.pitch = 1f;
        GameManager.MissedTap.SetActive(true);


    }


}
