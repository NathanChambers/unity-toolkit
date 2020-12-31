using System;

public interface INotificationService : IService {
	void Schedule(string title, string message, int utc, Action<string, Error> response);
	void Reschedule(string identifier, string title, string message, int utc, Action<string, Error> response);
	void IsScheduled(string identifier, Action<bool, Error> response);
	void Cancel(string identifier, Action<Error> response);
	void CancelAll(Action<Error> response);	
}
