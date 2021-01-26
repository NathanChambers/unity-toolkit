using UnityEngine;
using Toolkit;

public abstract class MonoBehaviourEx : MonoBehaviour {
    public virtual void Awake() {
        Globals.Inject(this);
    }

    public virtual void Start() {

    }

    public virtual void Update() {

    }

    public virtual void OnEnable() {

    }

    public virtual void OnDisable() {

    }
}