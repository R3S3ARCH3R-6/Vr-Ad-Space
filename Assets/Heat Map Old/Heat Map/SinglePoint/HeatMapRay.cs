using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shoots a ray out its front and sets a gaze point for shaders
/// </summary>
public class HeatMapRay : MonoBehaviour {
    [SerializeField]
    private Vector3 gazePoint;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000)) {
            gazePoint = hit.point;
        }

        Shader.SetGlobalVector("_GazePosition", new Vector4(gazePoint.x, gazePoint.y, gazePoint.z, 1));
	}
}
