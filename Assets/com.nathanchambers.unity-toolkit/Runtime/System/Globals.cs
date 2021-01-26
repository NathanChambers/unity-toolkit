using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {
    public interface IGlobal {}

    public static class Globals {
        private static Dictionary<Type, IGlobal> globals = new Dictionary<Type, IGlobal>();

        public static void Register<T>() where T : IGlobal, new() {
            Register(new T());
        }

        public static void Register<T>(T system) where T : IGlobal {
            Requires.NotNull(system);
            Requires.True(globals.ContainsKey(typeof(T)) == false);

            globals.Add(typeof(T), system);
        }

        public static void LoadScriptableObject<T>(string resourcePath) where T : UnityEngine.Object, IGlobal {
            Requires.True(string.IsNullOrWhiteSpace(resourcePath) == false);
            Requires.True(globals.ContainsKey(typeof(T)) == false);

            var resource = Resources.Load<T>(resourcePath);
            Requires.NotNull(resource, $"Unable to load resource. ({typeof(T).Name}) {resourcePath}");

            var instance = ScriptableObject.Instantiate(resource);
            globals.Add(typeof(T), instance);
        }

        public static void LoadPrefab<T>(string resourcePath, HideFlags hideFlags = HideFlags.HideAndDontSave) where T : UnityEngine.Object, IGlobal {
            Requires.True(string.IsNullOrWhiteSpace(resourcePath) == false);
            Requires.True(globals.ContainsKey(typeof(T)) == false);

            var resource = Resources.Load<GameObject>(resourcePath);
            Requires.NotNull(resource, $"Unable to load resource. ({typeof(T).Name}) {resourcePath}");

            GameObject go = GameObject.Instantiate(resource);
            go.name = typeof(T).Name;
            go.hideFlags = hideFlags;
            UnityEngine.Object.DontDestroyOnLoad(go);
            globals.Add(typeof(T), go.GetComponent<T>());
        }

        public static void Create<T>(HideFlags hideFlags = HideFlags.HideAndDontSave) where T : UnityEngine.MonoBehaviour, IGlobal {
            Requires.True(globals.ContainsKey(typeof(T)) == false);

            GameObject go = new GameObject();
            go.name = typeof(T).Name;
            go.hideFlags = hideFlags;
            globals.Add(typeof(T), go.AddComponent<T>());
        }

        public static void Inject<T>(T obj) {
            Requires.NotNull(obj);

            var objType = obj.GetType();
            var fields = objType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            foreach(var field in fields) {
                var type = field.FieldType;
                if(typeof(IGlobal).IsAssignableFrom(type) == false) {
                    continue;
                }

                Requires.True(globals.ContainsKey(type) == true, $"Failed to find dependency. {typeof(T).Name} requires {type.Name}");
                field.SetValue(obj, globals[type]);
            }
        }
    }
}