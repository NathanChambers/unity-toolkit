using System;
using UnityEngine;

public class UntiyRemoteSettingService : IRemoveSettingService {

	public void Sync(Action<Error> response) {
		int count = RemoteSettings.GetCount();
		Debug.LogFormat("[Unity Remove Settings] {0} remote values found", count);
		response.Invoke(null);
	}

	public bool GetBool(string key, bool defaultValue = false) {
		return RemoteSettings.GetBool(key, defaultValue);
	}
	public float GetFloat(string key, float defaultValue = 0.0f) {
		return RemoteSettings.GetFloat(key, defaultValue);
	}
	public int GetInt(string key, int defaultValue = 0) {
		return RemoteSettings.GetInt(key, defaultValue);
	}
	public string GetString(string key, string defaultValue = "") {
		return RemoteSettings.GetString(key, defaultValue);
	}
}