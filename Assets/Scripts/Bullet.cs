using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D _rb => GetComponent<Rigidbody2D>();
    public float damage = 0.5f;
    
    void Update() => transform.right = _rb.velocity;
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("Ennemy") && other.gameObject.name != "Destroyer") {
            Destroy(gameObject);
        }
    }
}
