using System;
public interface IRemoveSettingService : IService {
	void Sync(Action<Error> response);
	bool GetBool(string key, bool defaultValue = false);
	float GetFloat(string key, float defaultValue = 0.0f);
	int GetInt(string key, int defaultValue = 0);
	string GetString(string key, string defaultValue = "");
}