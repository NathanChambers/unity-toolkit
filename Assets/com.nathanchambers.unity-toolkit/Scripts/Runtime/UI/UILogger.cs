using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {

    public class UILogger : MonoBehaviour {
        public TMPro.TMP_Text uiLog;

        public void Awake() {
            Debug.Assert(uiLog != null);
            Application.logMessageReceived += LogHandler;
            uiLog.text = string.Empty;
        }

        private void LogHandler(string message, string stackTrace, LogType type) {
            uiLog.text = message;
        }
    }

}