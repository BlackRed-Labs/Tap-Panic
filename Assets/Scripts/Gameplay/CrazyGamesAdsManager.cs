using UnityEngine;
using CrazyGames;

public class CrazyGamesAdsManager : MonoBehaviour
{

    public static CrazyGamesAdsManager Instance;
   

    void Awake()
    {
        Instance = this;
    }


    public void ShowMidgameAd(System.Action OnMidgameAd)
    {
        if (!CrazySDK.IsAvailable) return;

        CrazySDK.Ad.RequestAd(
            CrazyAdType.Midgame,
            () => { },
            (error) => { Debug.LogWarning("Ad error: " + error); OnMidgameAd.Invoke(); },
            () => { OnMidgameAd.Invoke(); }
        );
    }

    public void ShowRewardedAd(System.Action onRewarded)
    {
        if (!CrazySDK.IsAvailable) return;

        CrazySDK.Ad.RequestAd(
            CrazyAdType.Rewarded,
            () => { Time.timeScale = 0; },
            (error) => { Debug.LogWarning("Ad error: " + error); Time.timeScale = 1; },
            () => { Time.timeScale = 1; onRewarded?.Invoke(); }
        );
    }

   

}