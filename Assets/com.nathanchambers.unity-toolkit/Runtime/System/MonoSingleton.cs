using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour {
    private static bool valid = false;
    private static T instance = null;

    public static T Instance {
        get {
            if (valid == false) {
                Debug.LogErrorFormat("No instance of {0} exists", typeof(T).Name);
                return null;
            }
            return instance;
        }
    }

    public static bool InstanceExists {
        get {
            return valid;
        }
    }

    public virtual void Awake() {
        if (valid == true) {
            Debug.LogWarningFormat("Second singleton instance of {0} at {1}. This instance has been ignored.", typeof(T).Name, Helpers.GameObjectPath(gameObject));
            return;
        }

        instance = this as T;
        valid = true;
    }

    public virtual void OnApplicationQuit() {
        if (instance != this) {
            return;
        }
    }

    public virtual void OnDestroy() {
        if (instance != this) {
            return;
        }

        instance = null;
        valid = false;
    }
}