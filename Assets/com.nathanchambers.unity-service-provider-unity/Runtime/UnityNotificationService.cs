//Ref: https://docs.unity3d.com/Packages/com.unity.mobile.notifications@1.0/manual/index.html

using System;
using System.Collections;
using UnityEngine;

#if UNITY_ANDROID
using Unity.Notifications.Android;
#elif UNITY_IOS
using Unity.Notifications.iOS;
#endif

public class UnityNotificationService : INotificationService {

	private static DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

	public static DateTime UTCToDateTime(double utc) {
		DateTime utcDateTime = epochStart.AddSeconds(utc);
		return utcDateTime;
	}

#if UNITY_ANDROID
	private const string androidNotificationChannelMainID = "main";

	public UnityNotificationService() {
		AndroidNotificationChannel channel = new AndroidNotificationChannel {
			Id = androidNotificationChannelMainID,
			Name = "Main Channel",
			Importance = Importance.High,
			Description = "Generic Notifications"
		};
		AndroidNotificationCenter.RegisterNotificationChannel(channel);
	}

	public void Schedule(string title, string message, int utc, Action<string, Error> response) {
		AndroidNotification notification = new AndroidNotification();
		notification.Title = title;
		notification.Text = message;
		notification.FireTime = UTCToDateTime(utc);

		string identifier = AndroidNotificationCenter.SendNotification(notification, androidNotificationChannelMainID).ToString();
		response.Invoke(identifier, null);
	}

	public void Reschedule(string identifier, string title, string message, int utc, Action<string, Error> response) {
		int notificationId = -1;
		int.TryParse(identifier, out notificationId);
		if(notificationId == -1) {
			return;
		}

		NotificationStatus status = AndroidNotificationCenter.CheckScheduledNotificationStatus(notificationId);
		AndroidNotification notification = new AndroidNotification();
		notification.Title = title;
		notification.Text = message;
		notification.FireTime = UTCToDateTime(utc);

		if(status == NotificationStatus.Scheduled) {
			AndroidNotificationCenter.UpdateScheduledNotification(notificationId, notification, androidNotificationChannelMainID);
		} else {
			identifier = AndroidNotificationCenter.SendNotification(notification, androidNotificationChannelMainID).ToString();	
		}

		response.Invoke(identifier, null);
	}

	public void IsScheduled(string identifier, Action<bool, Error> response) {
		bool scheduled = true;
		if(AndroidNotificationCenter.CheckScheduledNotificationStatus(int.Parse(identifier)) != NotificationStatus.Scheduled) {
			scheduled = false;
		}

		response.Invoke(scheduled, null);
	}

	public void Cancel(string identifier, Action<Error> response) {
		int notificationId = -1;
		int.TryParse(identifier, out notificationId);
		if(notificationId == -1) {
			return;
		}

		AndroidNotificationCenter.CancelNotification(notificationId);
		response.Invoke(null);
	}

	public void CancelAll(Action<Error> response) {
		AndroidNotificationCenter.CancelAllNotifications();
		response.Invoke(null);
	}

#elif UNITY_IOS

	private bool permission = false;

	public IEnumerator AuthenticateIOS() {
#if !UNITY_EDITOR
		using (var req = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, true)) {
			while (!req.IsFinished) {
				yield return null;
			};
			permission = req.Granted;
		}
#endif
		yield break;
	}

	public void Schedule(string title, string message, int utc, Action<string, Error> response) {
		if(permission == false) {
			response.Invoke(string.Empty, new Error("Not authorised"));
			return;
		}

		var timeTrigger = new iOSNotificationTimeIntervalTrigger() {
			TimeInterval = Epoch.UTCToTimeSpan(utc),
			Repeats = false
		};

		var notification = new iOSNotification() {
			Title = title,
			Body = message,
			ShowInForeground = true,
			ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
			CategoryIdentifier = "category_a",
			ThreadIdentifier = "thread1",
			Trigger = timeTrigger,
		};

		iOSNotificationCenter.ScheduleNotification(notification);

		response.Invoke(notification.Identifier, null);
	}

	public void Reschedule(string identifier, string title, string message, int utc, Action<string, Error> response) {
		iOSNotificationCenter.RemoveScheduledNotification(identifier);
		Schedule(title, message, utc, response);
	}

	public void IsScheduled(string identifier, Action<bool, Error> response) {
		var notifications = iOSNotificationCenter.GetScheduledNotifications();
		if(notifications == null || notifications.Length <= 0) {
			response.Invoke(false, null);
			return;
		}

		bool found = false;
		foreach(var notification in notifications) {
			if(notification.Identifier != identifier) {
				continue;
			}

			found = true;
			break;
		}

		response.Invoke(found, null);
	}

	public void Cancel(string identifier, Action<Error> response) {
		iOSNotificationCenter.RemoveScheduledNotification(identifier);
		response.Invoke(null);
	}

	public void CancelAll(Action<Error> response) {
		iOSNotificationCenter.RemoveAllScheduledNotifications();
		response.Invoke(null);
	}

#else
	
	public void Schedule(string title, string message, int utc, Action<string, Error> response) {
		//No Implementation
	}

	public void Reschedule(string identifier, string title, string message, int utc, Action<string, Error> response) {
		//No Implementation
	}

	public void IsScheduled(string identifier, Action<bool, Error> response) {
		//No Implementation
	}

	public void Cancel(string identifier, Action<Error> response) {
		//No Implementation
	}

	public void CancelAll(Action<Error> response) {
		//No Implementation
	}
#endif
}