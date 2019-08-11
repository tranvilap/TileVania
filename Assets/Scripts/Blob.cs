using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blob : MonoBehaviour {
    [SerializeField] Collider2D bodyCollider;
    [SerializeField] LayerMask whatisPlayer;
    Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        if (!bodyCollider.IsTouchingLayers(whatisPlayer)) { return; }
        else
        {
            Attack();
        }
    }

    private void Attack()
    {
        player.GetAttacked(GetComponent<Enemy>().Damage);
    }

}
