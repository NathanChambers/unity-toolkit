using System;
using UnityEngine;

public static class Epoch {

	public const int Minute = 60;
	public const int Hour = Minute * 60;
	public const int Day = Hour * 24;
	public const int Week = Day * 7;

	private static DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

	public static DateTime EpochStart {
		get {
			return epochStart;
		}
	}

	public static double Now {
		get {
			return (DateTime.UtcNow - epochStart).TotalSeconds;
		}
	}

	public static int NowRounded {
		get {
			return (int)Now;
		}
	}

	public static DateTime UTCToDateTime(double utc) {
		DateTime utcDateTime = epochStart.AddSeconds(utc);
		return utcDateTime;
	}

	public static TimeSpan UTCToTimeSpan(double utc) {
		return TimeSpan.FromSeconds((UTCToDateTime(utc) - System.DateTime.UtcNow).TotalSeconds);
	}

	public static double Elapsed(double timestamp) {
		double diff = Now - timestamp;
		if (diff < 0) {
			diff = -diff;
		}
		return diff;
	}

	public static double Elapsed(double lhs, double rhs) {
		double diff = rhs - lhs;
		if (diff < 0) {
			diff = -diff;
		}
		return diff;
	}
}