using System;

public interface IGroupsService : IService {
	 void UpdateGroups(Action<IGroup[], Error> response);
	 void CreateGroup(string groupId, Action<IGroup, Error> response);
	 void InviteUser(IGroup group, IUser user, Action<bool, Error> response);
	 void FindGroup(string groupId, Action<IGroup, Error> response);
	 void FindUser(string displayName, Action<IUser, Error> response);
}
