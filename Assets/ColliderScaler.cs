using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderScaler : MonoBehaviour
{
    // Use this for initialization
    void Start ()
    {
        var renderer = GetComponent<SpriteRenderer>();
        print(renderer.size);
        foreach (var collider in GetComponents<BoxCollider2D>())
        {
            print("collider " + collider.size);
            collider.size = new Vector2(renderer.size.x, collider.size.y);
            print("collider " + collider.size);
        }
    }

    // Update is called once per frame
    void Update ()
    {

    }
}
