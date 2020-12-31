using UnityEngine;

public static class TimeFormatter {
    public readonly static int[] intervals = {
        Constants.TIME_YEAR,
        Constants.TIME_WEEK,
        Constants.TIME_DAY,
        Constants.TIME_HOUR,
        Constants.TIME_MINUTE,
        Constants.TIME_SECOND,
    };

    public readonly static string[] replacementKeys = {
        "{y}",
        "{w}",
        "{d}",
        "{h}",
        "{m}",
        "{s}",
    };

    public const string Countdown_DHM = "{d}D {h}H {m}M";
    public const string Countdown_HM = "{h}H {m}M";

    public static string Format(float seconds, string format) {
        return Format(Mathf.CeilToInt(seconds), format);
    }

    public static string Format(int seconds, string format) {
        for(int i = 0; i < intervals.Length; ++i) {
            int count = seconds / intervals[i];
            string replacementKey = replacementKeys[i];

            if(format.IndexOf(replacementKey) < 0) {
                continue;
            }

            format = format.Replace(replacementKey, count.ToString("00"));
            seconds = seconds % intervals[i];
        }
        return format;
    }
}