using System;
using System.Collections.Generic;
using UnityEngine.Analytics;


public class UnityAnalyticService : IAnalyticService {

    public void TriggerUserEvent(string eventId, Dictionary<string, object> data, Action<Error> response) {
        AnalyticsResult result = AnalyticsEvent.Custom(eventId, data);
        if(response == null) {
            return;
        }
        
        if(result != AnalyticsResult.Ok) {
            response.Invoke(new Error(result.ToString()));
        }   else {
            response.Invoke(null);
        }
    }
}