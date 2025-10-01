using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class Reward : MonoBehaviour
{   
     #if UNITY_EDITOR
            string adUnitId = "unused";
        #elif UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/5224354917";
        #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/1712485313";
        #else
            string adUnitId = "unexpected_platform";
        #endif

        private RewardedAd _rewardedAd;

    public void LoadRewardAd()
    {
        LoadAd();
    }
    public void ShowRewardAd()
    {
        // ServerSideVerification(_rewardedAd);
        ShowAd(_rewardedAd);
    }

    public void Start()
    {
        // Initialize Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
        });

        // Create our request used to load the ad.
    }

    void LoadAd()
    {
        var adRequest = new AdRequest();

        // Send the request to load the ad.
        RewardedAd.Load(adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.LogError($"RewardedAd load failed: {error}");
                _rewardedAd = null;
                return;
            }
                // The ad loaded successfully.

            _rewardedAd = ad; // ★ 콜백에서 필드에 저장
            ListenToAdEvents(_rewardedAd);
            Debug.Log("광고로드 성공");
        });
    }
    // void ServerSideVerification(RewardedAd rewardedAd)
    //     {
    //         // [START ssv]
    //         // Create and pass the SSV options to the rewarded ad.
    //         var options = new ServerSideVerificationOptions
    //         {
    //             CustomData = "SAMPLE_CUSTOM_DATA_STRING"
    //         };

    //         rewardedAd.SetServerSideVerificationOptions(options);
    //         // [END ssv]
    //     }

        void ShowAd(RewardedAd rewardedAd)
        {
            // [START show_ad]
            if (rewardedAd != null && rewardedAd.CanShowAd())
            {
                rewardedAd.Show((GoogleMobileAds.Api.Reward reward) =>
                {
                    Debug.Log($"User earned reward: {reward.Amount} {reward.Type}");
                    // The ad was showen and the user earned a reward.
                }

                );
            }
            else MonoBehaviour.print("광고로드가 아직 안됐습니다.");
            // [END show_ad]]
        }

        void ListenToAdEvents(RewardedAd rewardedAd)
        {
            // [START ad_events]
            rewardedAd.OnAdPaid += (AdValue adValue) =>
            {
                // Raised when the ad is estimated to have earned money.
            };
            rewardedAd.OnAdImpressionRecorded += () =>
            {
                // Raised when an impression is recorded for an ad.
            };
            rewardedAd.OnAdClicked += () =>
            {
                // Raised when a click is recorded for an ad.
            };
            rewardedAd.OnAdFullScreenContentOpened += () =>
            {
                // Raised when the ad opened full screen content.
            };
            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                // Raised when the ad closed full screen content.
            };
            rewardedAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                // Raised when the ad failed to open full screen content.
            };
            // [END ad_events]]
        }

        void DestroyAd(RewardedAd rewardedAd)
        {
            // [START destroy_ad]
            if (rewardedAd != null)
            {
                rewardedAd.Destroy();
            }
            // [END destroy_ad]]
        }

        void ReloadAd(RewardedAd rewardedAd)
        {
            // [START reload_ad]
            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                // Reload the ad so that we can show another as soon as possible.
                var adRequest = new AdRequest();
                RewardedAd.Load(adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
                {
                    if (error != null)
                {
                    Debug.LogError($"Reload failed: {error}");
                    _rewardedAd = null;
                    return;
                }   
                     _rewardedAd = ad;
                    ListenToAdEvents(_rewardedAd);
                    ReloadAd(_rewardedAd);
                    Debug.Log("RewardedAd reloaded.");
                    // Handle ad loading here.
                });
            };
            // [END reload_ad]]
        }

}
