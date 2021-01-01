using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIEventHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public Action<PointerEventData> PointerEnter;
    public Action<PointerEventData> PointerExit;

    public void OnPointerEnter(PointerEventData eventData) {
        if (PointerEnter != null) {
            PointerEnter.Invoke(eventData);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (PointerExit != null) {
            PointerExit.Invoke(eventData);
        }
    }
}