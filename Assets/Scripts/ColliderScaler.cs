using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderScaler : MonoBehaviour
{
    // Use this for initialization
    void Start ()
    {
        var renderer = GetComponent<SpriteRenderer>();
        foreach (var collider in GetComponents<BoxCollider2D>())
        {
            collider.size = new Vector2(renderer.size.x, collider.size.y);
        }
    }

    // Update is called once per frame
    void Update ()
    {

    }
}
