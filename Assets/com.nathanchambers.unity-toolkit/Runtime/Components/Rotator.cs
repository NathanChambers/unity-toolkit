using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {

    public class Rotator : MonoBehaviourEx {
        public Vector3 axis;
        public float speed;

        private float angle = 0.0f;

        public override void Start() {
            base.Start();
            angle = UnityEngine.Random.Range(0, 360);
        }

        public override void Update() {
            base.Update();
            angle += Time.deltaTime * speed;
            var rotation = transform.rotation;
            transform.rotation = Quaternion.AngleAxis(angle, axis);;
        }
    }

}