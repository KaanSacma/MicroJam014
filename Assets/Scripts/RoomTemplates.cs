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
    
    private Transform _parent;
    
    public float waitTime;
    public float maxWaitTime;
    public bool spawnedBoss = false;
    public GameObject[] bossRooms;
    private GameObject _bossroom;
    
    private void SpawnBossRoom()
    {
        if (rooms[rooms.Count - 1].name.Contains("T")) {
            _bossroom = Instantiate(bossRooms[0], rooms[rooms.Count - 1].transform.position, Quaternion.identity);
        } else if (rooms[rooms.Count - 1].name.Contains("B")) {
            _bossroom = Instantiate(bossRooms[1], rooms[rooms.Count - 1].transform.position, Quaternion.identity);
        } else if (rooms[rooms.Count - 1].name.Contains("L")) {
            _bossroom = Instantiate(bossRooms[2], rooms[rooms.Count - 1].transform.position, Quaternion.identity);
        } else if (rooms[rooms.Count - 1].name.Contains("R")) {
            _bossroom = Instantiate(bossRooms[3], rooms[rooms.Count - 1].transform.position, Quaternion.identity);
        }
        _bossroom.transform.parent = _parent;
    }
    
    private void Start()
    {
        _parent = GameObject.FindGameObjectWithTag("GridWorld").transform;
        maxWaitTime = waitTime;
    }

    private void Update()
    {
        if (waitTime <= 0 && spawnedBoss == false) {
            for (int i = 0; i < rooms.Count; i++) {
                if (i == rooms.Count - 1) {
                    SpawnBossRoom();
                    Destroy(rooms[rooms.Count - 1]);
                    spawnedBoss = true;
                }
            }
        } else {
            waitTime -= Time.deltaTime;
        }
    }
}
