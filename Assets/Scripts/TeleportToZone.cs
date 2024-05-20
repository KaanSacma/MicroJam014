using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportToZone : MonoBehaviour
{
    public bool isRun = false;
    public bool isTuto = false;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) {
            if (isRun) {
                other.transform.position = new Vector3(-3, 100, 0);
            } else if (isTuto) {
                other.transform.position = new Vector3(-3, -715, 0);
            }
        }
    }
}
