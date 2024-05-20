using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{

    private GameObject _parentOther;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.parent == gameObject.transform.parent 
            || other.CompareTag("Player") 
            || other.CompareTag("Ennemy")
            || other.CompareTag("Bullet")
            || other.CompareTag("EnemyBullet"))
            return;
        if (other.gameObject.name == "Walls" && other.gameObject.transform.parent && other.gameObject.transform.parent.name.Contains("(Clone)")) {
            _parentOther = other.gameObject.transform.parent.gameObject;
            Destroy(_parentOther);
        } else {
            Destroy(other.gameObject);
        }
        
    }
}
