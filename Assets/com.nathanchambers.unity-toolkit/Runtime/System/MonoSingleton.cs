using UnityEngine;

namespace Toolkit {
    public class MonoSingleton<T> : MonoBehaviourEx where T : MonoBehaviour{
        private static T instance = null;
        public static T Instance {
            get {
                return instance;
            }
        }

        public override void Awake() {
            base.Awake();
            if(instance != null) {
                throw new CriticalErrorException($"Multiple instances of {typeof(T).Name} found");
            }
            instance = this as T;
        }
    }
}