using System.Collections.Generic;
namespace Toolkit {
    public class ValueBase {
        protected ValueBase() {
            // Discard
        }
    }

    public class Value<T> : ValueBase {
        public T value;
        public Value(T _value) {
            value = _value;
        }
    }

    public class ValueCollection {
        Dictionary<string, ValueBase> values = new Dictionary<string, ValueBase>();

        public void Set(string id, ValueBase value) {
            if(values.ContainsKey(id) == false) {
                values.Add(id, value);
            } else {
                values[id] = value;
            }
        }

        public bool Exists(string id) {
            return values.ContainsKey(id);
        }

        public Value<T> Get<T>(string id, T defaultValue) {
            if(values.ContainsKey(id) == false) {
                return new Value<T>(defaultValue);
            }
            return values[id] as Value<T>;
        }
    }
}