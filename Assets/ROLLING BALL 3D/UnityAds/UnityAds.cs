using UnityEngine;
using System.Collections;

//#if UNITY_ADS
using UnityEngine.Advertisements;
//#endif

public class UnityAds : MonoBehaviour {
    public static UnityAds Instance;
    [HideInInspector] public int coinReward = 50;
    //public AudioClip soundReward;

    [Header("UNITY AD SETUP")]
    public string UNITY_ANDROID_ID = "1486550";
    public string UNITY_IOS_ID = "1486551";
    public bool isTestMode = true;

    private void Awake()
    {
        if (UnityAds.Instance)
            Destroy(gameObject);
    }

    // Use this for initialization
    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        string gameId = "XXX";
#if UNITY_IOS
		gameId = UNITY_IOS_ID;
#elif UNITY_ANDROID
        gameId = UNITY_ANDROID_ID;
#endif

        if (Advertisement.isSupported)
        {
            Advertisement.Initialize(gameId, isTestMode);
        }
    }

    //#if UNITY_ADS

    public bool isRewardedAdReady()
    {
        return Advertisement.IsReady("rewardedVideo");
    }
    
    public void ShowRewardVideo() {
        ShowRewardedAd();
    }


    public int b4showFullAds = 3;
    [HideInInspector]
    public int counter = 0;

    public void ShowNormalAd()
    {
        counter++;
        if (counter >= (SetupGameValue.Instance? SetupGameValue.Instance.showAdWhenGameoverTimes : b4showFullAds))
        {
            if (Advertisement.IsReady())
            {
                Advertisement.Show();

                counter = 0;
                //if (interstitial.IsLoaded ())
                //	interstitial.Show ();

                //Debug.LogError("SHOW FULL AD");
            }
        }

    }


    private void ShowRewardedAd()
    {
        //Debug.LogError("ShowRewardAds FFF");
        if (Advertisement.IsReady("rewardedVideo"))
        {
            //Debug.LogError("ShowRewardAds RRR");
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
            counter = 0;    //reset counter to avoid user watch many video
        }
    }




    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");

                GlobalValue.StoreDiamond += (SetupGameValue.Instance ? SetupGameValue.Instance.rewardedGemWatchAd : coinReward);
                SoundManager.PlaySfx(SoundManager.Instance.soundRewarded);

                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
    }
    //#endif
}
