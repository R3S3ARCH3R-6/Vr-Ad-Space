using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Website: https://www.youtube.com/watch?v=XO-E6QaTniQ (spawner and destroyer)
/// </summary>

public class ObjectControl : MonoBehaviour
{
    private float objTime = 30f;
    
    void FixedUpdate()
    {

        if(objTime > 0) {
            objTime -= Time.deltaTime;
            if(objTime <= 0) {
                TimeDestroy();
            }
        }
    }

    void TimeDestroy() {
        Destroy(this.gameObject);
    }

    void ObjDestroy(GameObject otherObjects) {
        Destroy(otherObjects);
    }
}
