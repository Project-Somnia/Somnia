using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class Topbanner : MonoBehaviour
{   
    BannerView _bannerView;

    // These ad units are configured to always serve test ads.
    public void RequestBanner()
    {   
        // These ad units are configured to always serve test ads.
        #if UNITY_EDITOR
            string adUnitId = "unused";
        #elif UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/6300978111";
        #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
        #else
            string adUnitId = "unexpected_platform";
        #endif

    
        // Clean up banner ad before creating a new one.
        if (_bannerView != null)
        {
            _bannerView.Destroy();
        }

        AdSize adaptiveSize =
                AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        _bannerView = new BannerView(adUnitId, adaptiveSize, AdPosition.Top);

        // Register for ad events.
        _bannerView.OnBannerAdLoaded += OnBannerAdLoaded;
        _bannerView.OnBannerAdLoadFailed += OnBannerAdLoadFailed;

        AdRequest adRequest = new AdRequest();

        // Load a banner ad.
        _bannerView.LoadAd(adRequest);
    }

    private void OnBannerAdLoaded()
    {
        Debug.Log("Banner view loaded an ad with response : "
                 + _bannerView.GetResponseInfo());

        Debug.Log($"Ad Height: {_bannerView.GetHeightInPixels()}, Width: {_bannerView.GetWidthInPixels()}");
    }

    private void OnBannerAdLoadFailed(LoadAdError error)
    {
        Debug.LogError("Banner view failed to load an ad with error : "
                + error);
    }

//   public void CreateBannerView()
//   {
//       Debug.Log("Creating banner view");

//       // If we already have a banner, destroy the old one.
//       if (_bannerView != null)
//       {
//           DestroyAd();
//       }

//       // Create a 320x50 banner at top of the screen
//       _bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Top);
//   }

// public void LoadAd()
// {
//     // create an instance of a banner view first.
//     if(_bannerView == null)
//     {
//         CreateBannerView();
//     }

//     // create our request used to load the ad.
//     var adRequest = new AdRequest();

//     // send the request to load the ad.
//     Debug.Log("Loading banner ad.");
//     _bannerView.LoadAd(adRequest);
// }

public void DestroyAd()
{
    if (_bannerView != null)
    {
        Debug.Log("Destroying banner view.");
        _bannerView.Destroy();
        _bannerView = null;
    }
}



    // Start is called before the first frame update
    void Start()
    {
       // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
        }); 
        this.RequestBanner();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
