using UnityEngine;

namespace Toolkit {

    [ExecuteInEditMode]
    public class MainLight : MonoBehaviourEx {
        public override void Start() {
            base.Start();
            Shader.SetGlobalVector("_MainLightDir", -transform.forward);
        }

        public override void Update() {
            base.Update();
            Shader.SetGlobalVector("_MainLightDir", -transform.forward);
        }
    }

}