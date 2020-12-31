using System.Runtime.CompilerServices;
public class Error {

    private Error encapulated = null;
    public string Message {
        get;
        private set;
    }

    public Error(string message, Error encapulated = null) {
        this.encapulated = encapulated;
        this.Message = message;
    }

    public override string ToString() {
        if (encapulated == null) {
            return StackMessage();
        } else {
            return $"{StackMessage()}\n{encapulated.ToString()}";
        }
    }

    private string StackMessage([CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) {
        return $"({filePath}:{lineNumber}) {Message}";
    }

}