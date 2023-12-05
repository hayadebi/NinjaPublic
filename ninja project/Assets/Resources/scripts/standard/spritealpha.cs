using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spritealpha : MonoBehaviour
{
    private SpriteRenderer thissprite = null;
    public SpriteRenderer targetsprite;
    // Start is called before the first frame update
    void Start()
    {
        thissprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(thissprite && thissprite.color.a != targetsprite.color.a)
        {
            Color tmp = thissprite.color;
            tmp.a = targetsprite.color.a;
            thissprite.color = tmp;
        }
    }
}
