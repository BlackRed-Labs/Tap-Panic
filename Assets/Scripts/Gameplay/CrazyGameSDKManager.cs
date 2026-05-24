using UnityEngine;
using CrazyGames; // Namespace from SDK

public class CrazyGamesManager : MonoBehaviour
{
    public static CrazyGamesManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

     public void OnGameplayBegins()
    {
        if (CrazySDK.IsAvailable)
            CrazySDK.Game.GameplayStart(); // player is now playing
    }

     public void OnGamePaused()
    {
        if (CrazySDK.IsAvailable)
            CrazySDK.Game.GameplayStop(); // not playing right now
    }



}
