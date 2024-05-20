using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] public Vector3 spawnPosition;
    [SerializeField] private float damage;
    [SerializeField] private float health;
    [SerializeField] private float speed;
    [SerializeField] private float shootDistance;
    [SerializeField] private float detectionDistance;
    
    [Header("Gun")]
    [SerializeField] private Transform ennemyGun;
    [SerializeField] private float gunDistance = 1.0f;
    [SerializeField] public float gunDamage = 0.5f;
    [SerializeField] private Transform ennemyCenter;
    
    [Header("Shoot")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private float shootSpeed;
    [SerializeField] private float shootCooldown = 1.0f;
    
    private Animator enemyAnimator;
    private GameObject player;
    private float currentCooldown = 0.0f;
    
    private void FlipEnemy(Vector3 direction)
    {
        if (direction.x < 0) {
            transform.localScale = new Vector3(-2, 2, 1);
        } else {
            transform.localScale = new Vector3(2, 2, 1);
        }
    }
    
    private void GunRotation()
    {
        Vector3 direction = player.transform.position - ennemyCenter.position;
        
        ennemyGun.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        ennemyGun.position = ennemyCenter.position + Quaternion.Euler(0, 0, angle + 10) * new Vector3(gunDistance, 0, 0);
    }
    
    private void Shoot(Vector3 direction)
    {
        gameObject.GetComponent<AudioSource>().Play();
        GameObject bulletInstance = Instantiate(bullet, ennemyGun.position, Quaternion.identity);
        bulletInstance.GetComponent<Bullet>().damage = gunDamage;
        bulletInstance.GetComponent<Rigidbody2D>().velocity = direction.normalized * shootSpeed;
        Destroy(bulletInstance, 2.0f);
    }
    
    private void Start()
    {
        transform.position = spawnPosition;
        player = GameObject.FindGameObjectWithTag("Player");
        enemyAnimator = GetComponent<Animator>();
    }
    
    private void FixedUpdate()
    {
        currentCooldown += Time.deltaTime;
        
        if (gameObject.GetComponent<Rigidbody2D>().velocity.magnitude > 0) {
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        
        if (player != null && health > 0) {
            Vector3 directionPlayer = player.transform.position - transform.position;
            Vector3 directionSpawn = spawnPosition - transform.position;
            float distancePlayer = Vector3.Distance(player.transform.position, transform.position);
            var raycastHit = Physics2D.Raycast(transform.position, directionPlayer, detectionDistance);
            
            GunRotation();
            
            if (raycastHit.collider != null && raycastHit.collider.CompareTag("Player")) {
                FlipEnemy(directionPlayer);
                Debug.DrawLine(transform.position, raycastHit.point, Color.red);
                if (distancePlayer <= shootDistance) {
                    enemyAnimator.SetBool("IsRunning", false);
                    if (currentCooldown >= shootCooldown) {
                        enemyAnimator.SetBool("IsShooting", true);
                        currentCooldown = 0.0f;
                        Shoot(directionPlayer);
                        enemyAnimator.SetBool("IsShooting", false);
                    }
                } else if (distancePlayer <= detectionDistance) {
                    enemyAnimator.SetBool("IsShooting", false);
                    enemyAnimator.SetBool("IsRunning", true);
                    transform.position += directionPlayer.normalized * speed * Time.deltaTime;
                }
            } else if (transform.position != spawnPosition) {
                enemyAnimator.SetBool("IsShooting", false);
                enemyAnimator.SetBool("IsRunning", true);
                transform.localScale = new Vector3(2, 2, 1);
                transform.position += directionSpawn.normalized * speed * Time.deltaTime;
            } else {
                enemyAnimator.SetBool("IsShooting", false);
                enemyAnimator.SetBool("IsRunning", false);
                transform.localScale = new Vector3(2, 2, 1);
            }
        }
    }
    
    private IEnumerator TakeDamage(float damage)
    {
        health -= damage;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.69f, 0.24f, 0.32f, 1);
        yield return new WaitForSeconds(0.1f);
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("EnemyBullet")) {
            return;
        }
        
        if (other.gameObject.CompareTag("Bullet")) {
            Destroy(other.gameObject);
            StartCoroutine(TakeDamage(player.GetComponent<PlayerSystem>().bulletDamage));
        }
        
        if (health <= 0) {
            enemyAnimator.SetBool("IsDead", true);
            Destroy(gameObject, 2.0f);
        }
    }
}
