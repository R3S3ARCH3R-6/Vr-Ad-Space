using UnityEngine;

/*
 * Moves the sprite object to a given position. Position is set by components of the DrawLine class.
 */
public class MoveSprite : MonoBehaviour
{

    public Vector3 destination = new Vector3(0, 0, 0); //The point at which our sprite will move to.
    float speed = 5000; //The speed with which the sprite moves.
    
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
    }
}
