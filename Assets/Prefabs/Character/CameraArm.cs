using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraArm : MonoBehaviour {
    public new GameObject camera;
    private float maxDistance;

    void Start() {
        maxDistance = (transform.position - camera.transform.position).magnitude;
    }

    void Update() {
        var start = transform.position;
        var ray = new Ray(start, -transform.forward);
        var hit = Physics.SphereCast(ray, 0.1f, out var hitInfo, maxDistance + 0.5f);

        var distance = hit ? hitInfo.distance : maxDistance;

        camera.transform.position = start + transform.forward * -distance;
    }
}
