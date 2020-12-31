using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CriticalErrorException : UnityException {

    private string file = string.Empty;
    private string method = string.Empty;
    private int line = 0;
    private string message = string.Empty;

    public CriticalErrorException(string _message, [CallerFilePath] string _file = null, [CallerMemberName] string _method = null, [CallerLineNumber] int _line = 0) : base(_message) {
        file = _file;
        method = _method;
        line = _line;

        string path = file.Substring(file.LastIndexOf("Assets"));
        message = string.Format("{0}\nTrace: {1} (at {2}:{3})", _message, method, path, line);

        OnException();
    }

    private void OnException() {
        ApplicationControl.Instance.CriticalError(this);
    }

    public override string Message {
        get {
            return message;
        }
    }

    public override string ToString() {
        return message;
    }
    
}