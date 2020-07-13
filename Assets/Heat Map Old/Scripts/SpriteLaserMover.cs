using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this is based heavily off DrawLine.cs from the oil rig

//this is a script designed to move adil's sprite around on the screen
    //this also functions as a simple ray
public class SpriteLaserMover : MonoBehaviour
{
    public GameObject adilSprite;
    public Vector3 offset = new Vector3(0.0f, 4.59f, 0.66f); //offset so the laser instantiates in the proper location
    public float tolerance = 0.0f;

    private GameObject spriteCopy;  //copy of adil's script
    private LineRenderer laser; //laser beam

    private Vector3 origin; //origin for the laser (not needed?)
    private Vector3 destination = Vector3.zero;    //destination of the laser beam
    private Vector3 mousePos; //stores the mouse's position (moves the head around with the mouse)
    private Vector3 changePos = Vector3.zero;

    private float distance = 0.0f;
    private float maxDistance = 20f;    //for raycast and laser
    private float radius = 0.5f;      //use if you decide to use spherecast
    private float counter;    //number of positions for the laser
    private float lineSpeed = 1f;   //speed the laser is drawn
    private bool drawLine = true; //controls when the laser should be drawn

    private Quaternion rayAngle = Quaternion.identity;
    
    
    void Start()
    {
        spriteCopy = Instantiate(adilSprite);

        laser = GetComponent<LineRenderer>(); //Gets the LineRenderer attached to this object
        origin = this.transform.position + offset; //Calculate the origin point by adding an offset to it
        laser.startWidth = 0.1f;
        laser.endWidth = 0.1f;

    }
    
    void FixedUpdate()
    {
        Vector3 newDestination = Vector3.zero; //The latest point where our ray collided with an object.
        float newDistance = 0f; //The distance between newDestination and origin.
        bool hasHit = false; //Records if the spherecast has hit anything.
        GameObject objectFocusedOn = null; //The object the head is looking at
        float t = Time.deltaTime;

        laser.SetPosition(0, origin);   //enables the spherecast to move with the object when the object moves

        //Check to see if this object's position has changed. If so, change the origin point of the array.
        if (transform.hasChanged)
        {
            origin = this.transform.position; //+ offset;
            transform.hasChanged = false; //transform.hasChanged must be reset to false.
        }
        //performs the basic ray code from the FixedUpdate method in DrawLine.cs
        rayFire(origin, newDestination, newDistance, hasHit);
        laserFire();
        
        //GetPositions()    --> need x,y,z of GetPosition

        //mouseMovement();
    }

    //this code makes the object with this script move with the mouse
    void mouseMovement()
    {
        mousePos = Input.mousePosition;
        mousePos.z = 4.0f;
        transform.position = Camera.main.ScreenToWorldPoint(mousePos);
    }

    void rayFire(Vector3 origin, Vector3 newDestination, float newDistance, bool hasHit)
    {
        Vector3 rayOrigin = transform.position;

        //Here we cast a ray against all colliders in the scene to see where it hits.
        Ray ray = new Ray(origin, transform.forward);       //ray must be created in update so it can move
        RaycastHit hit;     //must be made with the ray to detect collisions (must be made in the same location as ray)

        //Fire a spherecast from the ray. If it hits something, we set newDestination and newDistance to the point that the ray hit and the distance between that point and the origin.
        //format: Physics.Raycast(Ray ray, out RaycastHit hitName, float rayDistance)
        //means if the ray's raycast (the "out" makes the RaycastHit the hitter for the ray) hits an object at the given distance
        if (Physics.SphereCast(ray, radius, out hit, maxDistance))
        {
            newDestination = hit.point;
            newDistance = hit.distance;
            spriteCopy.transform.position = hit.point; //hit.point must be used (gives the object the same position as the RaycastHit)
                                                       //don't use hit.transform.position
            hasHit = true;
        }
        //If the spherecast hits nothing, newDistance and newDestination will be set to a point 50 units from the origin.
        else
        {
            newDistance = maxDistance;
            newDestination = ray.GetPoint(20f);
            spriteCopy.transform.position = rayOrigin + (transform.forward * maxDistance);
        }
    }

    void laserFire()
    {
        Vector3 spriteVector = spriteCopy.transform.position;

        if (drawLine)
        {
            counter += 0.1f / lineSpeed;    //helps create draw effect ("Animation")
            float x = Mathf.Lerp(0, distance, counter);

            Vector3 pointAlongLine = x * Vector3.Normalize(destination - origin) + origin;
            //get unit vector in a desired direction (x), then you want to mult. by a link and then get the starting position
            //helps with animation (helps us get the pt. val. per frame)

            laser.SetPosition(1, spriteVector);
        }
    }

}
