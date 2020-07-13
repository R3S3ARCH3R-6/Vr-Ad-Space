using System.Collections.Generic;
using UnityEngine;
//using ViveSR.anipal.Eye;
/// <summary>
/// Fires a spherecast from between the eyes of the student/head, and creates visual representations of where they're looking.
/// Allows for simulated movement from premade data, or moving the head via eye tracking data.
/// </summary>
/// This code is a derivative of the DrawLine.cs found in the oil rig
public class DrawLine : MonoBehaviour
{
    /// <summary>
    /// The LineRenderer that will create the laser.
    /// </summary>
    private LineRenderer laser;
    /// <summary>
    /// used as an incrementer element for drawing the line
    /// </summary>
    private float counter;
    /// <summary>
    /// the distance between the hitpoint and the ray's origin
    /// </summary>
    private float distance = 0.0f;
    /// <summary>
    /// the radius of the spherecast.
    /// </summary>
    public float radius = 0.5f;
    /// <summary>
    /// The max distance where the spherecast can detect 
    /// </summary>
    public float maxDistance = 50f;
    /// <summary>
    /// The point of origin for the ray and laser.
    /// </summary>
    private Vector3 origin;
    /// <summary>
    /// An offset that places the ray and laser at a proper origin between the eyes. Used if gazeMode is set to head. 
    /// </summary>
    public Vector3 offset = new Vector3(0.0f, 4.59f, 0.66f);
    /// <summary>
    /// The destination of the laser
    /// </summary>
    private Vector3 destination = Vector3.zero;
    /// <summary>
    /// A quaternion that stores the rotation between the origin and destination.
    /// </summary>
    private Quaternion rayAngle = Quaternion.identity;
    private float lineSpeed = 1f;
    Vector3 changePos = Vector3.zero;
    /// <summary>
    /// Determines whether or not a visible line (the laser) will be drawn to represent the position the player is looking at.
    /// </summary>
    public bool drawLine = true;
    /// <summary>
    /// The tolerance for the difference between the previous destination position and the current one
    /// </summary>
    public float tolerance = 0.0f;
    /// <summary>
    /// The filename where our raw head movement data will be written.
    /// </summary>
    string filename;
    public enum GazeMode
    {
        /// <summary>
        /// JSONDATA means this head's movement is determined by input from a data file
        /// LIVE_EYE means this head's movement is determined by the eye tracking data of the user.
        /// </summary>
        JSONDATA,
        LIVE_EYE
    }
    /// <summary>
    /// Determines where the input comes from.
    /// </summary>
    public GazeMode gazeMode;
    [SerializeField]
    private int maxPoints = 1000;
    [SerializeField]
    private List<Vector4> gazePoints;
    [SerializeField]
    private Shader shader;
    [SerializeField]
    [Range(0, 30)]
    private int pointsPerSecond = 10;
    float timeSincePing = 0;
    float pingInterval;

    void Start()
    {
        laser = GetComponent<LineRenderer>(); //Gets the LineRenderer attached to this object
        origin = this.transform.position + offset; //Calculate the origin point by adding an offset to it
        laser.startWidth = 0.1f;
        laser.endWidth = 0.1f;
        gazePoints = new List<Vector4>(1000);
        for (int i = 0; i < maxPoints; i++)
            gazePoints.Add(new Vector4(0, 0, 0, 1));

        pingInterval = 1 / (float)pointsPerSecond;
        SetShaderValues();
    }

    /*
     * Contains the majority of the code for this class; creates and fires a ray, draws the laser, and sets up the heatmap.
     */
    void FixedUpdate()
    {
        Vector3 newDestination; //The latest point where our ray collided with an object.
        float newDistance; //The distance between newDestination and origin.
        bool hasHit = false; //Records if the spherecast has hit anything.
        GameObject objectFocusedOn = null; //The object the head is looking at
        float t = Time.deltaTime;

        laser.SetPosition(0, origin);   //enables the spherecast to move with the object when the object moves

        //Check to see if this object's position has changed. If so, change the origin point of the array.
        if (transform.hasChanged)
        {
            origin = this.transform.position + offset;
            transform.hasChanged = false; //transform.hasChanged must be reset to false.
        }
        //Here we cast a ray against all colliders in the scene to see where it hits.
        Ray ray = new Ray(origin, transform.forward);
        RaycastHit hit;


        //Fire a spherecast from the ray. If it hits something, we set newDestination and newDistance to the point that the ray hit and the distance between that point and the origin.
        if (Physics.SphereCast(ray, radius, out hit, maxDistance))
        {
            newDestination = hit.point;
            newDistance = hit.distance;

            //Record the gameObject that is being focused on.
            objectFocusedOn = hit.transform.gameObject;

            hasHit = true;
        }
        //If the spherecast hits nothing, newDistance and newDestination will be set to a point 50 units from the origin.
        else
        {
            newDistance = maxDistance;
            newDestination = ray.GetPoint(50f);

        }

        /*Check to see if the difference between newDistance and distance is greater or less than the tolerance.*/
        if (Mathf.Abs(newDistance - distance) > tolerance)
        {
            rayAngle = onChanged(newDistance, newDestination).normalized; //onChanged changes destination/distance to newDestination/newDistance + returns rotation quaternion of the ray

            if (hasHit) //Only runs if our spherecast has hit something.
            {
                setupHeatmap(); //Sets up our heatmap and sends it the latest rotation quaternion
                //Next, we record what and where the player was looking at.
                OutputRawGazeData o = new OutputRawGazeData();
                if (filename == null) //If our filename is null, that means our file doesn't exist, which means we need to create it.
                {
                    filename = o.CreateFile();
                }
                //We create a new object of type GazeData (see OutputRawGazeData.cs for more info) and populate its values.
                GazeData g = new GazeData();
                g.xV = destination.x;
                g.yV = destination.y;
                g.zV = destination.z;
                g.xQ = rayAngle.x;
                g.yQ = rayAngle.y;
                g.zQ = rayAngle.z;
                g.wQ = rayAngle.w;
                g.t = t;
                g.objectLookedAt = objectFocusedOn.ToString();
                o.WriteGazeData(g, filename); //We write the GazeData to the file.
            }
        }
        //If drawLine = true, we draw the laser (a visual representation of the ray).
        DrawLaser();
    }

    /// <summary>
    /// Creates a visual representation of the ray. May be turned on and off using public variable drawLine.
    /// </summary>
    void DrawLaser()
    {
        if (drawLine)
        {
            counter += 0.1f / lineSpeed;    //helps create draw effect ("Animation")
            float x = Mathf.Lerp(0, distance, counter);

            Vector3 pointAlongLine = x * Vector3.Normalize(destination - origin) + origin;
            //get unit vector in a desired direction (x), then you want to mult. by a link and then get the starting position
            //helps with animation (helps us get the pt. val. per frame)

            laser.SetPosition(1, pointAlongLine);
        }
    }

    /// <summary>
    /// Changes the value of global variables distance and destination to newDistance and newDestination.
    /// </summary>
    /// <param name="newDistance">The value that global variable distance is being changed to.</param>
    /// <param name="newDestination">The value that global variable destination is being changed to.</param>
    /// <returns>A quaternion representing the angle between them.</returns>
    Quaternion onChanged(float newDistance, Vector3 newDestination)
    {
        distance = newDistance;
        destination = newDestination;
        return rayAngle.normalized;
    }

    /// <summary>
    /// Runs LaserHeatMapColorChange and sends the necessary values to the class.
    /// </summary>
    void setupHeatmap()
    {
        LaserHeatmapColorChange greenColor = GetComponent<LaserHeatmapColorChange>();
        greenColor.greenDraw(destination, rayAngle);
    }

    private void OnDestroy()
    {
        //We need to make sure we properly end the output file if this object is destroyed. So, we use this class to do so.
        if (filename != null)
        {
            OutputRawGazeData o = new OutputRawGazeData();
            o.EndFile(filename);
        }

    }

    void SetShaderValues()
    {
        Shader.SetGlobalInt("_Gaze_Length", maxPoints);
        Shader.SetGlobalVectorArray("_GazePositions", gazePoints.ToArray());
    }

    //The following is commented out due to being unnecessary.
    /* Calculates and returns a quaternion representing the rotation between two vectors. 
     * This is needed for getting the quaternion needed in the class LaserHeatmapColorChange.
     * Calculation for result.w taken from https://www.euclideanspace.com/maths/geometry/rotations/conversions/angleToQuaternion/index.htm
    Quaternion calculateQuaternionRotation(Vector3 u, Vector3 v)
    {

        //Calculate the dot product of u and v. We use this to help find the angle between the vectors. 
        float dotProduct = Vector3.Dot(u, v);

        //Check to see if vectors are the same or parallel. If so, we return an identity.
        if (dotProduct == 1.0f)
        {
            return Quaternion.identity;
        }

        //Check to see if vectors are perpendicular. If so, the dot product will be 0.
        else if (dotProduct == 0)
        {
            return new Quaternion(-u.z, u.y, u.x, 0);
        }

        else
        {
            Quaternion result = new Quaternion(0f, 0f, 0f, 0f);

            //The axis of rotation is the unit vector perpendicular to the plane formed by two vectors, so we can use the cross product to find it.
            Vector3 axisOfRotation = Vector3.Cross(u, v);

            //We find the angle between the two vectors using cos(x) = (u . v) / (||u|| * ||v||).
            //Debug
            float angle = Mathf.Acos(dotProduct / (u.magnitude * v.magnitude));
            float angleW = angle / 2f;
            //The x, y, and z values of our quaternion are the same as our axis of rotation.
            result.x = axisOfRotation.x;
            result.y = axisOfRotation.y;
            result.z = axisOfRotation.z;

            //Because the angle x between u and v can be found with x = cos^-1((u . v) / (||u|| * ||v||)), and the w of a quaternion is cos(x/2), we can get w with the following:
            result.w = Mathf.Sqrt(u.sqrMagnitude * v.sqrMagnitude) + dotProduct;
            return result.normalized;
        }
    }
    */

}
