using UnityEngine;
using UnityEngine.SceneManagement;
using CrazyGames;
using System.Collections;
using TMPro; // ← this gives access to the SDK

public class CrazySDKInitializer : MonoBehaviour
{
    // Set this in the Inspector to your main scene name
    [SerializeField] private int nextSceneBuildIndex;
    public TMP_Text loadingText;

    void Start()
    {
        if (CrazySDK.IsAvailable)
        {
            // We are on CrazyGames (or Editor/localhost) — init the SDK
            CrazySDK.Init(OnSDKReady);
        }
        else
        {
            // Not on CrazyGames (e.g. Android build) — skip and load game
            LoadNextScene();
        }
    }

    private void OnSDKReady()
    {
        Debug.Log("CrazyGames SDK is ready!");
        LoadNextScene(); // Now safe to load the game
    }

    private void LoadNextScene()
    {
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        
        AsyncOperation operation =
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            yield return null;
        }

        // ✅ WAIT FOR TITLE ANIMATION HERE
        yield return new WaitForSecondsRealtime(1.6f); // adjust to your animation length

        // scene is ready → wait or auto switch
        operation.allowSceneActivation = true;
    }


}
