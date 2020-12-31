using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image)), RequireComponent(typeof(RectTransform))]
public class UIFixedTiledSize : MonoBehaviour {

    public bool MatchX = true;
    public bool MatchY = true;

    private Image image = null;
    private RectTransform rectTransform = null;

    private Image Image {
        get {
            if(image == null) {
                image = GetComponent<Image>();
            }
            return image;
        }
    }

    private RectTransform RectTransform {
        get {
            if(rectTransform == null) {
                rectTransform = GetComponent<RectTransform>();
            }
            return rectTransform;
        }
    }

    public void Resize() {
        Sprite sprite = Image.sprite;
        if(sprite == null) {
            return;
        }

        RectTransform.ForceUpdateRectTransforms();
        if(MatchX == true) {
            float closestSize = sprite.texture.width * (int)(RectTransform.rect.width / sprite.texture.width);
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, closestSize);
        }

        if(MatchY == true) {
            float closestSize = sprite.texture.height * (int)(RectTransform.rect.height / sprite.texture.height);
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, closestSize);
        }
    }
}


#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(UIFixedTiledSize))]
public class UIFixedTiledSizeEditor : UnityEditor.Editor {
    private UIFixedTiledSize self = null;
    public override void OnInspectorGUI() {
        if(self == null) {
            self = target as UIFixedTiledSize;
        }

        if(GUILayout.Button("Resize") == true) {
            self.Resize();
        }
        base.OnInspectorGUI();
    }
}
#endif//UNITY_EDITOR