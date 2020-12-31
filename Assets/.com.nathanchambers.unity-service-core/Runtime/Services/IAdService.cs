using System;

public interface IAdService : IService {

    public enum AdResult {
        FINISHED,
        SKIPPED,
        ERROR,
    }

    public enum AdType {
        INTERSTITIAL,
        REWARDED,
    }

    bool IsReady(AdType type);
    void Show(AdType type, Action<AdResult> _adEvent);
}