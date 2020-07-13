using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveSR.anipal.Eye;

/// <summary>
/// Shoots a ray out its front and sets a gaze point for shaders
/// </summary>
public class HeatMapHistoryRay : MonoBehaviour {
    public enum GazeMode {
        HEAD,
        EYE
    }
    [SerializeField]
    private int maxPoints = 1000;
    [SerializeField]
    private List<Vector4> gazePoints;
    [SerializeField]
    private Shader shader;
    [SerializeField]
    [Range(0, 30)]
    private int pointsPerSecond = 10;
    public GazeMode gazeMode;
    float timeSincePing = 0;
    float pingInterval;

	// Use this for initialization
	void Start () {
        gazePoints = new List<Vector4>(1000);
        for (int i = 0; i < maxPoints; i++)
            gazePoints.Add(new Vector4(0, 0, 0, 1));

        pingInterval = 1 / (float)pointsPerSecond;
        SetShaderValues();
	}
	
	// Update is called once per frame
	void Update () {
        pingInterval = 1 / (float)pointsPerSecond;
        timeSincePing += Time.deltaTime;
        if (timeSincePing > pingInterval) {
            if(gazeMode == (GazeMode)0) {
                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 1000)) {
                    Vector3 gazePoint = hit.point;
                    gazePoints.Add(new Vector4(gazePoint.x, gazePoint.y, gazePoint.z, 1));
                    if (gazePoints.Count > maxPoints) {
                        gazePoints.RemoveAt(0);
                    }
                }
            } else {
                if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
                        SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;
                bool isLeftEyeActive = false;
                bool isRightEyeAcitve = false;
                EyeData eyeData = new EyeData();
                if (SRanipal_Eye_Framework.Status == SRanipal_Eye_Framework.FrameworkStatus.WORKING)
                {
                    isLeftEyeActive = eyeData.verbose_data.left.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_ORIGIN_VALIDITY);
                    isRightEyeAcitve = eyeData.verbose_data.right.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_ORIGIN_VALIDITY);
                }
                else if (SRanipal_Eye_Framework.Status == SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT)
                {
                    isLeftEyeActive = true;
                    isRightEyeAcitve = true;
                }
                Vector3 GazeOriginCombinedLocal, GazeDirectionCombinedLocal = Vector3.zero;
                if (SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
                else if (SRanipal_Eye.GetGazeRay(GazeIndex.LEFT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
                else if (SRanipal_Eye.GetGazeRay(GazeIndex.RIGHT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
                Vector3 GazeDirectionCombined = Camera.main.transform.TransformDirection(GazeDirectionCombinedLocal);
                Ray ray = new Ray(Camera.main.transform.position - Camera.main.transform.up * 0.05f, GazeDirectionCombined);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, 1000)) {
                    Vector3 gazePoint = hit.point;
                    gazePoints.Add(new Vector4(gazePoint.x, gazePoint.y, gazePoint.z, 1));
                    if(gazePoints.Count > maxPoints) {
                        gazePoints.RemoveAt(0);
                    }
                }
            }

            //Shader.SetGlobalVector("_GazePosition", new Vector4(gazePoint.x, gazePoint.y, gazePoint.z, 1));
            //for (int i = 0; i < gazePoints.Count; i++) {
            //    Shader.SetGlobalVector("_GazePositions" + i.ToString(), gazePoints[i]);
            //}

            //Shader.SetGlobalVector("_GazePositions0", new Vector4(1, 1, 1, 1));

            //Shader.SetGlobalVectorArray("_GazePositions", gazePoints);
            SetShaderValues();

            timeSincePing = 0;
        }
	}

    void SetShaderValues() {
        Shader.SetGlobalInt("_Gaze_Length", maxPoints);
        Shader.SetGlobalVectorArray("_GazePositions", gazePoints.ToArray());
    }
}
