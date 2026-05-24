using CrazyGames;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CrazySDKLoadingScene : MonoBehaviour
{
    public int nextScenIndex;

    void Start()
    {
        int NextScene = SceneManager.GetActiveScene().buildIndex+1;
        if (nextScenIndex < 1)
        {
            Debug.LogError("Please provide a nextSceneindex in " + nextScenIndex);
            return;
        }

        if (CrazySDK.IsAvailable)
        {
            CrazySDK.Init(() =>
            {
                Debug.Log("CrazySDK initialized");
                SceneManager.LoadScene(NextScene);
            });
        }
        else
        {
            SceneManager.LoadScene(NextScene);
        }
    }
}
