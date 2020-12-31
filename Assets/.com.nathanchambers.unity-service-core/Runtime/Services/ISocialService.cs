using System;
using System.Collections.Generic;

public interface ISocialService : IService {
	void UpdateUserProfile(IUser user, Action<Error> response);
	void UpdateDisplayName(string displayName, Action<Error> response);
	void AddFriend(string displayName, Action<Error> response);
	void RemoveFriend(IUser user, Action<Error> response);
	void GetFriendsList(Action<List<IUser>, Error> response);
};