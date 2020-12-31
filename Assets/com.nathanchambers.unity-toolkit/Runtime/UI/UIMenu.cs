using UnityEngine;

namespace Toolkit {
    public abstract class UIMenu : MonoBehaviour {
        public string MenuID = string.Empty;
        public abstract void Initialise(UIMenu parent);
        public abstract void Activated();
        public abstract void Deactivated();
        public abstract void Cleanup();
    }
}