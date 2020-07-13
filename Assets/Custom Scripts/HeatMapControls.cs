using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

public class HeatMapControls : MonoBehaviour
{
    public HeatMapHistoryRay heatMapScript;

    // Start is called before the first frame update
    void Start()
    {
        heatMapScript.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One)) {
            heatMapScript.enabled = !heatMapScript.enabled;
        } 

    }
}
