using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class CountdownTimer : MonoBehaviour
{
    public static CountdownTimer Instance;

    private Label countDownLabel;
    private int currentCount;
    private Coroutine countdownCoroutine;
    public AudioSource CountdownSFX;

   
    private void Awake()
    {
        Instance = this;

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        countDownLabel = root.Q<Label>("CountDownNumber");
    }

    public void UpdateCountDown(int count)
    {
       
        currentCount = count;
        countDownLabel.text = currentCount.ToString();

        // Stop old coroutine if already running
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }

        countdownCoroutine = StartCoroutine(UpdateCountDownText());
    }

    IEnumerator UpdateCountDownText()
    {
        while (currentCount > 0)
        {
            CountdownSFX.PlayOneShot(CountdownSFX.clip);
            yield return new WaitForSeconds(0.9f);
            currentCount--;
            CountdownSFX.PlayOneShot(CountdownSFX.clip);
            CountdownSFX.pitch += 0.1f; // Increase pitch for each count
            countDownLabel.text = currentCount.ToString();
        }
        CountdownSFX.pitch = 1f; // Reset pitch after countdown
        gameObject.SetActive(false);
        GameManager.Instance.isGameStarted = true;
        GameManager.Instance.StartGame();
    }
}