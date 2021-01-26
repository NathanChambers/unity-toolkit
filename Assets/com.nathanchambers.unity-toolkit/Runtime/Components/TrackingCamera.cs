using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {

	public class TrackingCamera : MonoBehaviourEx {
		public Transform target;
		public Vector3 offset;

		public override void Start() {
			base.Start();
			if (target == null) {
				throw new ArgumentNullException("target", "Tracking Camera target is null");
			}
		}

		public override void Update() {
			base.Update();
			transform.position = target.position + offset;
		}
	}

}