using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {
    public Vector3 axis;
    public float speed;

    private float angle = 0.0f;

    public void Start() {
        angle = UnityEngine.Random.Range(0, 360);
    }

    public void Update() {
        angle += Time.deltaTime * speed;
        var rotation = transform.rotation;
        transform.rotation = Quaternion.AngleAxis(angle, axis);;
    }
}