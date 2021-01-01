using System;
using System.Collections;
using UnityEngine;

public class ApplicationControl : MonoSingleton<ApplicationControl> {
    public Action OnBackground;
    public Action OnForground;

    ////////////////////////////////////////////////////////////////////////////////

    private Coroutine criticalErrorCoroutine = null;
    private ILogger criticalLogger = new Logger(Debug.unityLogger.logHandler);

    ////////////////////////////////////////////////////////////////////////////////

    public override void OnApplicationQuit() {
        if (criticalErrorCoroutine != null) {
            return;
        }

        if (OnBackground != null) {
            OnBackground.Invoke();
        }
    }

    public void OnApplicationPause(bool pauseStatus) {
        if (criticalErrorCoroutine != null) {
            return;
        }

        if (pauseStatus == true && OnBackground != null) {
            OnBackground.Invoke();
        } else if (pauseStatus == false && OnForground != null) {
            OnForground.Invoke();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////

#if UNITY_EDITOR
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnRecompile() {
        if (Application.isPlaying == false) {
            return;
        }

        Debug.Log("Scripts re-compiled while application is running. Closing application");
        ApplicationControl.ForceClose();
    }
#endif//UNITY_EDITOR

    public void CriticalError<ExceptionType>(ExceptionType exception)where ExceptionType : Exception {
        if (criticalErrorCoroutine != null) {
            return;
        }

        criticalErrorCoroutine = StartCoroutine(CriticalErrorCoroutine(exception));
    }

    private IEnumerator CriticalErrorCoroutine(Exception exception) {
        yield return new WaitForEndOfFrame();

#if UNITY_EDITOR
        UnityEditor.EditorUtility.DisplayDialog(exception.GetType().Name, exception.ToString(), "Close");
#else
        criticalLogger.LogException(exception);
#endif

        ForceClose();
    }

    public static void ForceClose() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif//UNITY_EDITOR
    }
}