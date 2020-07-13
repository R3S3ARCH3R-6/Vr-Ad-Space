using UnityEngine;

//might be able to delete this

//website: https://www.youtube.com/watch?v=g2k8jWqlRiE
//Help with changing the alpha: https://answers.unity.com/questions/958370/how-to-change-alpha-of-a-sprite.html
public class RandomColorChanger : MonoBehaviour
{
    private SpriteRenderer sprite;

    public float speed = 0.05f;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        altLerpChange();
    }

    //based off Lerp from "ParticleColorChanger" and "RandomChanger" method from video
    void altLerpChange()
    {
        float t = Time.time * speed;
        Color color1 = new Color(0f, 0f, 1f, 0.55f);
        Color color2 = new Color(1f, 0f, 0f, 0.55f);
        sprite.color = Color.Lerp(color1, color2, t);
    }
}
