using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Toolkit {
	[RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
	public class GestureManager : MonoSingleton<GestureManager>, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler {

		public const int POINTER_CODE_LEFT_CLICK = -1;
		public const int POINTER_CODE_RIGHT_CLICK = -2;
		public const int POINTER_CODE_MIDDLE_CLICK = -3;

		public Action<PointerEventData> TouchDown;
		public Action<PointerEventData> TouchUp;
		public Action<PointerEventData> DragBegin;
		public Action<PointerEventData> Drag;
		public Action<PointerEventData> DragEnd;
		public Action<PointerEventData> Tap;

		public void OnPointerDown(PointerEventData eventData) {
			if (eventData.pointerCurrentRaycast.gameObject != gameObject) {
				return;
			}

			if (TouchDown != null) {
				TouchDown.Invoke(eventData);
			}
		}

		public void OnPointerUp(PointerEventData eventData) {
			if (TouchUp != null) {
				TouchUp.Invoke(eventData);
			}
		}

		public void OnPointerClick(PointerEventData eventData) {
			if (Tap != null) {
				Tap.Invoke(eventData);
			}
		}

		public void OnBeginDrag(PointerEventData eventData) {
			if (eventData.pointerCurrentRaycast.gameObject != gameObject) {
				return;
			}

			if (DragBegin != null) {
				DragBegin.Invoke(eventData);
			}
		}

		public void OnEndDrag(PointerEventData eventData) {
			if (DragEnd != null) {
				DragEnd.Invoke(eventData);
			}
		}

		public void OnDrag(PointerEventData eventData) {
			if (Drag != null) {
				Drag.Invoke(eventData);
			}
		}

	}
}