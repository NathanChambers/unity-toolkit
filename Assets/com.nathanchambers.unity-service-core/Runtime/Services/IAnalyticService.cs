using System;
using System.Collections.Generic;

public interface IAnalyticService : IService {
	void TriggerUserEvent(string eventId, Dictionary<string, object> data, Action<Error> response);
}