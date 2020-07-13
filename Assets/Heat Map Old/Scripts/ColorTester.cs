using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTester : MonoBehaviour
{
    SpriteRenderer coloredSprite;

    void Start()
    {
        coloredSprite = GetComponent<SpriteRenderer>();
    }

    
    void Update()
    {
        coloredSprite.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    }
}
