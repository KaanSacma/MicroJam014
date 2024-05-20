using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnEnemyInRoom : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemies;
    [SerializeField] private int _maxEnemies;
    [SerializeField] private int _minEnemies;
    [SerializeField] private Vector3 _spawnTopLeftCorner;
    [SerializeField] private Vector3 _spawnBottomRightCorner;
    private int _random;
    
    private void Start()
    {
        _random = Random.Range(_minEnemies, _maxEnemies);
        for (int i = 0; i < _random; i++) {
            int randomEnemy = Random.Range(0, _enemies.Length);
            Vector3 randomPosition = new Vector3(Random.Range(transform.position.x + _spawnTopLeftCorner.x, transform.position.x + _spawnBottomRightCorner.x), Random.Range(transform.position.y +_spawnBottomRightCorner.y, transform.position.y + _spawnTopLeftCorner.y), 0);
            GameObject enemy = Instantiate(_enemies[randomEnemy], randomPosition, Quaternion.identity);
            enemy.transform.parent = transform;
            if (enemy.GetComponent<SimpleEnemy>()) {
                enemy.GetComponent<SimpleEnemy>().spawnPosition = randomPosition;
                enemy.transform.position = randomPosition;
            }
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < transform.childCount; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
