using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomChanger : MonoBehaviour
{
    public Transform kitchen;
    public Transform bedroom;
    public Transform livingRoom;
    public Transform store;

    public GameObject player;

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Kitchen")) {
            player.transform.position = kitchen.transform.position;
        }

        if(other.gameObject.CompareTag("Living Room")) {
            player.transform.position = livingRoom.transform.position;
        }

        if(other.gameObject.CompareTag("Bedroom")) {
            player.transform.position = bedroom.transform.position;
        }

        if (other.gameObject.CompareTag("Store")) {
            player.transform.position = store.transform.position;
        }
    }

}
