using UnityEngine;

namespace Toolkit {

	public class FreeCamera : MonoBehaviourEx, IGlobal {

		public float velocity = 10.0f;
		public float sensitivity = 1.0f;

		private bool usingFreeCamera = false;
		private float positionScale = 1.0f;

		private bool cacheCursorVisible = false;
		private CursorLockMode cacheCursorLockState = CursorLockMode.None;
		private Vector3 cachePosition = Vector3.zero;
		private Quaternion cacheRotation = Quaternion.identity;

		public override void Awake() {
			base.Awake();
			Globals.Register(this);
		}

		public override void Update() {
			base.Update();

			if (Input.GetKeyDown(KeyCode.F10) == true) {
				usingFreeCamera = !usingFreeCamera;

				if (usingFreeCamera == true) {
					CacheValues();
				} else {
					RestoreValues();
				}

				Debug.LogFormat("Free Camera {0}", usingFreeCamera == true ? "Enabled" : "Disabled");
			}

			if (usingFreeCamera == true) {
				UpdateFreeCamera();
			}
		}

		private void UpdateFreeCamera() {

			bool shiftDown = Input.GetKey(KeyCode.LeftShift);

			float mouseX = Input.GetAxis(InputCode.LookYaw);
			float mouseY = Input.GetAxis(InputCode.LookPitch);
			Vector3 euler = transform.localRotation.eulerAngles;
			euler.x -= mouseY * sensitivity;
			euler.y += mouseX * sensitivity;
			transform.localRotation = Quaternion.Euler(euler);

			Vector3 positionDelta = Vector3.zero;
			positionDelta += transform.forward * Input.GetAxisRaw(InputCode.Depth);
			positionDelta += transform.right * Input.GetAxisRaw(InputCode.Horizontal);
			positionDelta += transform.up * Input.GetAxisRaw(InputCode.Vertical);

			positionScale += Input.GetAxisRaw(InputCode.ScrollWheel) * 5.0f;
			if (positionScale < 1.0f) {
				positionScale = 1.0f;
			}

			transform.localPosition = transform.localPosition + positionDelta.normalized * Time.unscaledDeltaTime * positionScale * (shiftDown ? 2.0f : 1.0f);
		}

		private void CacheValues() {
			cacheCursorVisible = Cursor.visible;
			cacheCursorLockState = Cursor.lockState;
			cachePosition = transform.position;
			cacheRotation = transform.rotation;

			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		private void RestoreValues() {
			Cursor.visible = cacheCursorVisible;
			Cursor.lockState = cacheCursorLockState;
			transform.position = cachePosition;
			transform.rotation = cacheRotation;
		}
	}

}