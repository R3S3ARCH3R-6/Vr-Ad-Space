using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Renderer : MonoBehaviour
{
    public List<GameObject> RenderableObjects;
    public Transform renderPos;
    private int ListLocation = 0;

    //Vector3 pageLoc = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update() {
        if(ListLocation > 3) {
            ListLocation = 0;
        }

        if(ListLocation < 0) {
            ListLocation = 3;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(gameObject.tag == "Page1") {
            Instantiate(RenderableObjects[ListLocation], renderPos.position, Quaternion.identity);
            ListLocation--;
        }

        if(gameObject.tag == "Page2") {
            Instantiate(RenderableObjects[ListLocation], renderPos.position, Quaternion.identity);
            ListLocation++;
        }
    }
}
