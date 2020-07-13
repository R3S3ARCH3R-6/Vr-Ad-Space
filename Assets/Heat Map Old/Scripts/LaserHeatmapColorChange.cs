using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//website for time delays: https://www.youtube.com/watch?v=ACDZ3W-stCE
//website for "WaitUntil" function: https://docs.unity3d.com/ScriptReference/WaitUntil.html
//second website for "WaitUntil": http://www.theappguruz.com/blog/the-mystery-of-waituntil-waitwhile-in-unity-5-3-revealed
//website for the sprites' transparency: https://answers.unity.com/questions/958370/how-to-change-alpha-of-a-sprite.html
public class LaserHeatmapColorChange : MonoBehaviour
{
    public GameObject greenLayer;

    private SpriteRenderer GSprite;

    Vector3 greenPos;

    bool callOnce = false;  //used to make sure the sprite is instantiated only 1 time
        //sprite cannot move, however when this var. is "true"

    void Start()
    {
        greenLayer.GetComponent<GameObject>();
        GSprite = greenLayer.GetComponent<SpriteRenderer>();
    }

    //draw circle where collision occurs --> make that the new start function
    public void greenDraw(Vector3 laserEndpoint, Quaternion rayAngle)
    {
        rayAngle = rayAngle.normalized;
        Vector3 movedPos = Vector3.zero; //used to follow the main sprite (shaky sprite and laser are made, however)
        //bool hasMoved = false;
        
        if(callOnce == false)
        {
            greenPos = laserEndpoint + new Vector3(0.0f, 0.0f, -0.2f);  //not sure if the offset is needed
            greenLayer = Instantiate(greenLayer, greenPos, rayAngle);
            //greenLayer.transform.localScale += new Vector3(5.0f, 5.0f, 0.0f);
            GSprite.color = Color.green;

            Color tempGreen = GSprite.GetComponent<SpriteRenderer>().color;
            tempGreen.a = 0.55f;

            Color tempBlue = Color.blue;
            tempBlue.a = 0.55f;

            GSprite.GetComponent<SpriteRenderer>().color = tempGreen;

            //lerp funct
            GSprite.color = Color.Lerp(GSprite.color, tempGreen, (Time.time * 0.05f));
            movedPos = greenPos;
            
            callOnce = true;
        }

        //makes a new sprite when the laserend moves and hits a collider object
        if (movedPos != (laserEndpoint + new Vector3(0.0f, 0.0f, -0.2f)))
        {
            StartCoroutine(TimeLapse());   
        }
    }

    IEnumerator TimeLapse()
    {
        yield return new WaitForSeconds(5f);
        callOnce=false;
    }

}
