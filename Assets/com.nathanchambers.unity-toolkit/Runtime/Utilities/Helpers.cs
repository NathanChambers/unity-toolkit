using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {

	public static class Helpers {
		public static string GameObjectPath(GameObject source) {
			if (source == null) {
				return string.Empty;
			}

			string path = source.name;
			Transform nextParent = source.transform.parent;
			while (nextParent != null) {
				path = string.Format("{0}/{1}", nextParent.name, path);
				nextParent = nextParent.parent;
			}

			return path;
		}

		public static bool RectContains(Vector2Int min, Vector2Int max, Vector2Int value) {
			if (value.x < min.x || value.x > max.x) {
				return false;
			}

			if (value.y < min.y || value.y > max.y) {
				return false;
			}

			return true;
		}
	}

}