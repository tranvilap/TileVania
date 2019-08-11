using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Springboard : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Rigidbody2D>())
        {
            collision.GetComponent<Rigidbody2D>().velocity += new Vector2(0f, 30f);
        }
    }
}
