using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.parent == gameObject.transform.parent || other.CompareTag("Player")) return;
        Destroy(other.gameObject);
    }
}
