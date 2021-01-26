using UnityEngine;

namespace Toolkit {

	public class UISafeArea : MonoBehaviourEx {
		private RectTransform rectTransform = null;
		private RectTransform RectTransform {
			get {
				if (rectTransform == null) {
					rectTransform = GetComponent<RectTransform>();
				}
				return rectTransform;
			}
		}

		public override void Start() {
			base.Start();
			float xAnchor = (Screen.safeArea.width / Screen.width) * 0.5f;
			float yAnchor = (Screen.safeArea.height / Screen.height) * 0.5f;
			RectTransform.anchorMin = new Vector2(0.5f - xAnchor, 0.5f - yAnchor);
			RectTransform.anchorMax = new Vector2(0.5f + xAnchor, 0.5f + yAnchor);
		}
	}
}