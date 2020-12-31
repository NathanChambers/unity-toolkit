using System;
using System.Collections.Generic;
using UnityEngine;

public interface ILocalService : IService {

	void GetUserData(IUser user, string key, Action<string, Dictionary<string, string>, Error> response);
	void SetUserData(IUser user, string key, Dictionary<string, string> data, Action<Error> response);
	void DeleteUserData(IUser user, string key, Action<Error> response);
	bool UserDataExists(IUser user, string key);
}