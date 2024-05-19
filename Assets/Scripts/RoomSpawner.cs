using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=UnityEngine.Random;

public class RoomSpawner : MonoBehaviour
{
    public int openingDirection; // T: 1 | B: 2 | R: 3 | L: 4
    
    private RoomTemplates _roomTemplates;
    private int _rand;
    public bool spawned = false;
    
    public float waitTime = 4f;

    private Transform _parent;
    private GameObject _room;
    
    void Start()
    {
        Destroy(gameObject, waitTime);
        _roomTemplates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        _parent = GameObject.FindGameObjectWithTag("GridWorld").transform;
        Invoke("SpawnRoom", 0.1f);
    }

    private void SpawnRoom()
    {
        if (spawned) return;
        
        switch (openingDirection) {
            case 1:
                _room = Instantiate(_roomTemplates.bottomRooms[Random.Range(0, _roomTemplates.bottomRooms.Length)], transform.position, Quaternion.identity);
                break;
            case 2:
                _room = Instantiate(_roomTemplates.topRooms[Random.Range(0, _roomTemplates.topRooms.Length)], transform.position, Quaternion.identity);
                break;
            case 3:
                _room = Instantiate(_roomTemplates.leftRooms[Random.Range(0, _roomTemplates.leftRooms.Length)], transform.position, Quaternion.identity);
                break;
            case 4:
                _room = Instantiate(_roomTemplates.rightRooms[Random.Range(0, _roomTemplates.rightRooms.Length)], transform.position, Quaternion.identity);
                break;
        }
        spawned = true;
        _room.transform.parent = _parent;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SpawnPoint") && other.GetComponent<RoomSpawner>()) {
            if (other.GetComponent<RoomSpawner>().spawned && spawned == false) {
                if (other.name == "Entry Room") return;
                Destroy(other.gameObject);
            } else if (spawned && other.GetComponent<RoomSpawner>().spawned == false) {
                if (name == "Entry Room") return;
                Destroy(gameObject);
            }
            spawned = true;
        }
    }
}
