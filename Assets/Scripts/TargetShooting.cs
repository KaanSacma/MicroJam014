using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetShooting : MonoBehaviour
{
    [SerializeField] private GameObject _target;
    [SerializeField] private float health = 1.5f;
    [SerializeField] private float timeToRespawn;
    
    private Animator targetAnimator => _target.GetComponent<Animator>();
    private GameObject player => GameObject.FindGameObjectWithTag("Player");
    private Rigidbody2D _rb => GetComponent<Rigidbody2D>();
    private BoxCollider2D _collider => GetComponent<BoxCollider2D>();

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(timeToRespawn);
        _target.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        targetAnimator.SetBool("IsDead", false);
        _collider.enabled = true;
        health = 1.5f;
    }

    private void Start()
    {
        _target = gameObject;
    }

    private void Update()
    {
        if (targetAnimator.GetBool("IsDead")) {
            _target.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
            _collider.enabled = false;
            StartCoroutine(Respawn());
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Bullet")) {
            health -= player.GetComponent<PlayerSystem>().bulletDamage;
            Destroy(other.gameObject);
        }
        
        if (health <= 0) {
            targetAnimator.SetBool("IsDead", true);
        }
    }
}
