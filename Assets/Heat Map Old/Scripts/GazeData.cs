using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Serializable class that holds the 3D vector point that the user was looking at, the quaternion representation of where the user is looking, the time at which they were looking at the object, 
/// and the object they're looking at.
/// </summary>
[System.Serializable]
public class GazeData
{
    //Below are the point values representing where the user is looking.
    public float xV;
    public float yV;
    public float zV;
    //Below are the quaternion values representing where the user is looking.
    public float xQ;
    public float yQ;
    public float zQ;
    public float wQ;
    public string objectLookedAt; //The object being looked at.
    public float t; //The time at which the object was looked at.
}

