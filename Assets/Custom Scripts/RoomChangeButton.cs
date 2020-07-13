using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomChangeButton : MonoBehaviour
{
    /*public Transform kitchen;
    public Transform bedroom;
    public Transform livingRoom;
    public Transform store;*/

    public Transform transition;
    public Transform spawn;

    public GameObject player;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Hands")) {
            player.transform.position = spawn.transform.position;
        }

        /*if (other.gameObject.CompareTag("Living Room")) {
            player.transform.position = livingRoom.transform.position;
        }

        if (other.gameObject.CompareTag("Bedroom")) {
            player.transform.position = bedroom.transform.position;
        }

        if (other.gameObject.CompareTag("Store")) {
            player.transform.position = store.transform.position;
        }*/
    }
}
