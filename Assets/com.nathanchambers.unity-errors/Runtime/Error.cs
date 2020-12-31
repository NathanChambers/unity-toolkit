using System.Runtime.CompilerServices;
public class Error {
    private string msg = string.Empty;
    private Error encapsulated = null;

    public string Message {
        get {
            if (encapsulated == null) {
                return msg;
            } else {
                return $"{msg}\n{encapsulated.Message}";
            }
        }
        private set {
            msg = value;
        }
    }

    public Error(string message, Error encapsulated = null) {
        this.encapsulated = encapsulated;
        this.msg = message;
    }

    public override string ToString() {
        return Message;
    }
}