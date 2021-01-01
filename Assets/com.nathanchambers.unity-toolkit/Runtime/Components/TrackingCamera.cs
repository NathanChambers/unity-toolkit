using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {

	public class TrackingCamera : MonoBehaviour {
		public Transform target;
		public Vector3 offset;

		public void Start() {
			if (target == null) {
				throw new ArgumentNullException("target", "Tracking Camera target is null");
			}
		}

		public void Update() {
			transform.position = target.position + offset;
		}
	}

}