using UnityEngine;

[ExecuteInEditMode]
public class MainLight : MonoBehaviour {
    void Start() {
        Shader.SetGlobalVector("_MainLightDir", -transform.forward);
    }

    void Update() {
        Shader.SetGlobalVector("_MainLightDir", -transform.forward);
    }
}