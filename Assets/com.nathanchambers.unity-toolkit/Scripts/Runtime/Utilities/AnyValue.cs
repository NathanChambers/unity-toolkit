using System.Collections.Generic;
namespace Toolkit {
    public class AnyValueBase {
        protected AnyValueBase() {
            // Discard
        }
    }

    public class AnyValue<T> : AnyValueBase {
        public T value;
        public AnyValue(T _value) {
            value = _value;
        }
    }

    public class AnyValueCollection {
        Dictionary<string, AnyValueBase> values = new Dictionary<string, AnyValueBase>();

        public void Set(string id, AnyValueBase value) {
            if(values.ContainsKey(id) == false) {
                values.Add(id, value);
            } else {
                values[id] = value;
            }
        }

        public bool Exists(string id) {
            return values.ContainsKey(id);
        }

        public AnyValue<T> Get<T>(string id, T defaultValue) {
            if(values.ContainsKey(id) == false) {
                return new AnyValue<T>(defaultValue);
            }
            return values[id] as AnyValue<T>;
        }
    }
}