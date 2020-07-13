using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRenderNew : MonoBehaviour
{

    public GameObject renderedObj;
    public Transform renderPosition;

    private GameObject trueObj;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Hands")) {
            trueObj = Instantiate(renderedObj, renderPosition.position, Quaternion.identity);
        }

    }
}
