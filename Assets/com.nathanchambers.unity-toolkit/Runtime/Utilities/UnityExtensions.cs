using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Toolkit {

	public static class ToolkitUnityExtensions {
		public static Vector3 ToVector3XY(this Vector2 input) {
			return new Vector3(input.x, input.y, 0.0f);
		}

		public static Vector3 ToVector3XZ(this Vector2 input) {
			return new Vector3(input.x, 0.0f, input.y);
		}

		public static Vector2 ToVector2XY(this Vector3 input) {
			return new Vector2(input.x, input.y);
		}

		public static Vector2 ToVector2XZ(this Vector3 input) {
			return new Vector2(input.x, input.z);
		}

		public static void LocalZero(this Transform transform) {
			transform.localPosition = Vector3.zero;
			transform.localScale = Vector3.one;
			transform.localRotation = Quaternion.identity;
		}

		public static string Color(this string str, Color colour) {
			return "<color=#" + ColorUtility.ToHtmlStringRGBA(colour) + ">" + str + "</color>";
		}

		public static Color GetRGB(this Color self, float alpha = 1.0f) {
			Color output = self;
			output.a = alpha;
			return output;
		}

		public static string Indent(this string str, int depth) {
			string indent = string.Empty;
			for (int i = 0; i < depth; ++i) {
				indent += "    ";
			}
			return indent + str;
		}

		public static float ToFloat(this Guid guid) {
			string guidStr = guid.ToString();
			return guidStr.GetHashCode();
		}

		public static bool TouchToVector3(this Plane plane, Vector2 touchPosition, out Vector3 worldPosition) {
			worldPosition = Vector3.zero;
			float enter = 0.0f;

			Ray ray = Camera.main.ScreenPointToRay(touchPosition);
			if (plane.Raycast(ray, out enter) == false) {
				return false;
			}

			worldPosition = ray.GetPoint(enter);
			return true;

		}

		public static bool TouchToVector3Int(this Plane plane, Vector2 touchPosition, out Vector3Int worldPosition) {
			worldPosition = Vector3Int.zero;
			float enter = 0.0f;

			Ray ray = Camera.main.ScreenPointToRay(touchPosition);
			if (plane.Raycast(ray, out enter) == false) {
				return false;
			}

			Vector3 hitPoint = ray.GetPoint(enter);
			worldPosition.x = (int)(hitPoint.x + 0.5f);
			worldPosition.y = (int)(hitPoint.y + 0.5f);
			worldPosition.z = (int)(hitPoint.z + 0.5f);
			return true;
		}

		public static void SetProgress(this RectTransform fillTransform, float perc) {
			fillTransform.anchorMax = new Vector2(perc, 1.0f);
		}

		public static void SetProgress(this Image fillImage, float perc) {
			fillImage.rectTransform.SetProgress(perc);
		}
	}

}