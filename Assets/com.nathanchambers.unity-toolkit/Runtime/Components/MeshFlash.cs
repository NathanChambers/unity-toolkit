using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Toolkit {
    public class MeshFlash {
        private GameObject gameObject;
        private Dictionary<Material, Color> initialColors = new Dictionary<Material, Color>();
        private float fadeInDuration = 0.0f;
        private float fadeOutDuration = 0.0f;
        private Color flashColor = Color.white;

        public MeshFlash(GameObject _gameObject, Color _color, float _fadeInDuration, float _fadeOutDuration) {
            gameObject = _gameObject;
            flashColor = _color;
            fadeInDuration = _fadeInDuration;
            fadeOutDuration = _fadeOutDuration;

            var renderers = gameObject.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers) {
                var material = renderer.material;
                if (initialColors.ContainsKey(material) == true) {
                    continue;
                }
                initialColors.Add(material, material.color);
            }
        }

        public IEnumerator Flash(bool continuous = false) {
            float timer = 0.0f;

            do {
                // Fade In
                while (timer < fadeInDuration) {
                    foreach (var kvp in initialColors) {
                        var material = kvp.Key;
                        material.EnableKeyword("_EMISSION");
                        material.SetInt("_EMISSION", 1);
                        material.SetColor("_EmissionColor", Color.Lerp(Color.black, flashColor, timer / fadeInDuration));
                    }
                    timer += Time.deltaTime;
                    yield return null;
                }

                // Fade out
                timer = fadeOutDuration;
                while (timer > 0.0f) {
                    foreach (var kvp in initialColors) {
                        var material = kvp.Key;
                        material.EnableKeyword("_EMISSION");
                        material.SetInt("_EMISSION", 1);
                        material.SetColor("_EmissionColor", Color.Lerp(Color.black, flashColor, timer / fadeOutDuration));
                    }
                    timer -= Time.deltaTime;
                    yield return null;
                }
            } while (continuous);

            Reset();
        }

        public void Interrupt() {
            Reset();
        }

        private void Reset() {
            foreach (var kvp in initialColors) {
                var material = kvp.Key;
                material.DisableKeyword("_EMISSION");
                material.SetInt("_EMISSION", 0);
                material.SetColor("_EmissionColor", Color.black);
            }
        }
    }
}