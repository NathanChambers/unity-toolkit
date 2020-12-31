using UnityEngine;

public static class Requires {
    public static void True(bool condition, string message = "") {
        Assert(condition == true, $"Assert! Failed true condition. {message}");
    }

    public static void SingletonExists(object instance, string message = "") {
        Assert(instance != null, $"Assert! Singleton doesn't exist. {message}");
    }

    public static void NotNull(object obj, string message = "") {
        Assert(obj != null, $"Assert! Null reference. {message}");
    }

    public static void ComponentReference(Component component, string message = "") {
        Assert(component != null, $"Assert! Component reference missing. {message}");
    }

    private static void Assert(bool condition, string message) {
        Debug.AssertFormat(condition, message);
    }

}