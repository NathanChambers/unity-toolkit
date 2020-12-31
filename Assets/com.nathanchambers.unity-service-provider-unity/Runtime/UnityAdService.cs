using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdService : IAdService, IUnityAdsListener {

    private string gameID = string.Empty;
    private Action<IAdService.AdResult> adEvent = null;

    public UnityAdService() {
        bool testMode = false;
#if DEBUG
        testMode = true;
#endif

#if !UNITY_ANDROID && !UNITY_IOS
        throw new UnityException("[UnityAdService] Platform not supported");
#endif

        Advertisement.AddListener(this);
        Advertisement.Initialize(gameID, testMode);
    }
    
    public void SetGameID(string gameID) {
        this.gameID = gameID;
    }

    public bool IsReady(IAdService.AdType type) {
        if(adEvent != null) {
            return false;
        }

        string placementId = GetPlacementID(type);
        if (string.IsNullOrEmpty(placementId) == true) {
            return false;
        }
        return Advertisement.IsReady(placementId);
    }

    public void Show(IAdService.AdType type, Action<IAdService.AdResult> _adEvent) {
        string placementId = GetPlacementID(type);
        if (string.IsNullOrEmpty(placementId) == true) {
            return;
        }

        adEvent = _adEvent;
        Advertisement.Show(placementId);
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult) {
        IAdService.AdResult result = IAdService.AdResult.ERROR;
        switch(showResult) {
            case ShowResult.Finished: result = IAdService.AdResult.FINISHED; break;
            case ShowResult.Skipped: result = IAdService.AdResult.SKIPPED; break;
        }
        
        if(adEvent != null) {
            adEvent.Invoke(result);
        }
        adEvent = null;
    }

    public void OnUnityAdsReady(string placementId) {
        Debug.LogFormat("[UnityAds] {0} placement ready", placementId);
    }

    public void OnUnityAdsDidError(string message) {
        Debug.LogFormat("[UnityAds] Error: {0}", message);
    }

    public void OnUnityAdsDidStart(string placementId) {
        Debug.LogFormat("[UnityAds] {0} placement started", placementId);
    }

    private string GetPlacementID(IAdService.AdType type) {
        switch (type) {
            case IAdService.AdType.INTERSTITIAL:
                return "video";
            case IAdService.AdType.REWARDED:
                return "rewardedVideo";
        }
        return string.Empty;
    }
}