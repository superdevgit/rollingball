using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class AskingContinueUI : MonoBehaviour {
    public Transform timerScale;
    public Button rewardedButton;
    public Button useGemButton;
    public float time = 3;
    float counting;
    public int diamondCost = 500;
    public Text diamondCostTxt;

    public int bonusWatchVideo = 100;

    public Text diamondBonusTxt;

    // Use this for initialization
    void Start () {
        counting = time;
        diamondCostTxt.text = diamondCost + "";

        useGemButton.interactable = diamondCost <= GlobalValue.StoreDiamond;

        if (UnityAds.Instance && UnityAds.Instance.isRewardedAdReady())
            rewardedButton.interactable = true;
        else
            rewardedButton.interactable = false;

        if (!useGemButton.interactable && !rewardedButton.interactable)
            Skip();
    }

    // Update is called once per frame
    void Update()
    {
        if(UnityAds.Instance && UnityAds.Instance.isRewardedAdReady())
        {
            diamondBonusTxt.text = "Bonus +" + bonusWatchVideo;
        }else
            diamondBonusTxt.text = "NO VIDEO AD NOW!";

        if (counting > 0)
        {
            counting -= Time.deltaTime;
            counting = Mathf.Clamp(counting, 0, time);
            timerScale.localScale = new Vector3(counting / time, 1, 1);
        }
        else
            MenuManager.Instance.ShowGameOverUI();

        if (UnityAds.Instance && UnityAds.Instance.isRewardedAdReady())
            rewardedButton.interactable = true;
        else
            rewardedButton.interactable = false;
    }

    public void UseDiamond()
    {
        if(GlobalValue.StoreDiamond >= diamondCost)
        {
            SoundManager.Click();
            GlobalValue.StoreDiamond -= diamondCost;
            Continue();
        }
    }

    public void Continue()
    {
        GameManager.Instance.Continue();
    }

    public void Skip()
    {
        SoundManager.Click();
        MenuManager.Instance.ShowGameOverUI();
    }


    public void ShowRewardedAd()
    {
        //Debug.LogError("ShowRewardAds FFF");
        if (Advertisement.IsReady("rewardedVideo"))
        {
            //Debug.LogError("ShowRewardAds RRR");
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);

            if (UnityAds.Instance)
                UnityAds.Instance.counter = 0;
        }
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                Continue();
                GlobalValue.StoreDiamond += bonusWatchVideo;
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
}
