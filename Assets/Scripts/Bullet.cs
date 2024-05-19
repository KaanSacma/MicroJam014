using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D _rb => GetComponent<Rigidbody2D>();
    void Update() => transform.right = _rb.velocity;
    
}
