using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Manager : MonoBehaviour
{
    GameObject[] renderedObjects;

    List<GameObject> renderObj;

    // Start is called before the first frame update
    void Start()
    {
        //renderedObjects = GameObject.FindGameObjectsWithTag("renderObj");
        
    }

    // Update is called once per frame
    void Update()
    {
        //renderedObjects
        //ArrayList<GameObject> up;

        renderedObjects = GameObject.FindGameObjectsWithTag("renderObj");
        if(renderedObjects.Length > 5) {
            Destroy(renderedObjects[0]);
            for(int i = 0; i < 4; i++) {    
                renderedObjects[i] = renderedObjects[i + 1];
            }
        }

        //Destroy(renderedObjects[0]);
    }
}
