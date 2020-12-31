using System;
using System.Collections.Generic;

public interface IMessageService : IService {
	void CreateMessage(string targetUserID, string title, string description, Dictionary<string, string> payload, Action<string, Error> response);
    void DestroyMessage(string messageUUID, Action<Error> response);
    void GetMessages(Action<IList<IMessage>, Error> response);
    void ReadMessage(string messageUUID, Action<Error> response);
}