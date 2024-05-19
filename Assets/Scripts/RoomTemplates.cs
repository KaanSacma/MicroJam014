using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTemplates : MonoBehaviour
{
    public GameObject[] bottomRooms;
    public GameObject[] topRooms;
    public GameObject[] leftRooms;
    public GameObject[] rightRooms;
    
    public List<GameObject> rooms;
    
    public float waitTime;
    private bool _spawnedBoss = false;
    public GameObject[] bossRooms;
    
    private void SpawnBossRoom()
    {
        if (rooms[rooms.Count - 1].name.Contains("T")) {
            Instantiate(bossRooms[0], rooms[rooms.Count - 1].transform.position, Quaternion.identity);
        } else if (rooms[rooms.Count - 1].name.Contains("B")) {
            Instantiate(bossRooms[1], rooms[rooms.Count - 1].transform.position, Quaternion.identity);
        } else if (rooms[rooms.Count - 1].name.Contains("L")) {
            Instantiate(bossRooms[2], rooms[rooms.Count - 1].transform.position, Quaternion.identity);
        } else if (rooms[rooms.Count - 1].name.Contains("R")) {
            Instantiate(bossRooms[3], rooms[rooms.Count - 1].transform.position, Quaternion.identity);
        }
    }

    private void Update()
    {
        if (waitTime <= 0 && _spawnedBoss == false) {
            for (int i = 0; i < rooms.Count; i++) {
                if (i == rooms.Count - 1) {
                    SpawnBossRoom();
                    Destroy(rooms[rooms.Count - 1]);
                    _spawnedBoss = true;
                }
            }
        } else {
            waitTime -= Time.deltaTime;
        }
    }
}
