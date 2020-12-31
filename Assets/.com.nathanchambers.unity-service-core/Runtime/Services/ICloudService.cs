using System;
using System.Collections.Generic;
using UnityEngine;

public interface ICloudService : IService {

	public enum DataScope {
		ERROR = 0,
		PUBLIC,
		PRIVATE
	}

	void GetUserData(IUser user, DataScope scope, Action<DataScope, Dictionary<string, string>, Error> response);
	void GetUserData(IUser user, DataScope scope, List<string> keys, Action<DataScope, Dictionary<string, string>, Error> response);
	void SetUserData(IUser user, DataScope scope, Dictionary<string, string> data, Action<DataScope, Error> response);
}