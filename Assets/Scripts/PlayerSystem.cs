using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerSystem : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float multipleClickMultiplier = 0;
    [SerializeField] private float maxMultipleClickMultiplier = 2.0f;
    
    [Header("Player")]
    [SerializeField] private GameObject player;
    [SerializeField] private float speed = 5f;
    [SerializeField] private int ammo = 3;
    [SerializeField] private float health = 3f;
    [SerializeField] private PlayerHud playerHud;
    
    [Header("Gun")]
    [SerializeField] private Transform playerGun;
    [SerializeField] private float gunDistance = 1.0f;
    [SerializeField] private Transform playerCenter;
    [SerializeField] private float refillTime = 0.50f;
    [SerializeField] private float shootTime = 0.15f;
    
    
    [Header("Bullet")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float destroyTime = 5.0f;
    
    private Animator _playerAnimator;
    private SpriteRenderer _playerSpriteRenderer;
    private Vector3 _targetPosition;
    private bool _canMove = false;
    private bool _isHoldingMove = false;
    private bool _canShoot = true;
    private int _maxAmmo = 3;
    private float _maxHealth = 3f;
    
    private void FlipPlayer()
    {
        Vector3 position = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = position - player.transform.position;
        
        if (direction.x < 0) {
            _playerSpriteRenderer.flipX = true;
        } else {
            _playerSpriteRenderer.flipX = false;
        }
    }
    
    private void GunRotation()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - playerCenter.position;
        
        playerGun.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        playerGun.position = playerCenter.position + Quaternion.Euler(0, 0, angle + 10) * new Vector3(gunDistance, 0, 0);
    }
    
    private IEnumerator RefillAmmo()
    {
        _playerAnimator.SetBool("IsAmmoEmpty", true);
        
        yield return new WaitForSeconds(refillTime);
        
        _playerAnimator.SetBool("IsAmmoEmpty", false);
        ammo = _maxAmmo;
        playerHud.UpdateAmmo((float)ammo / _maxAmmo);
        _canShoot = true;
    }
    
    private IEnumerator Shoot()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - playerCenter.position;
        
        FlipPlayer();
        _canShoot = false;
        ammo--;
        _playerAnimator.SetBool("IsShooting", true);
        GameObject bullet = Instantiate(bulletPrefab, playerGun.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().velocity = direction.normalized * bulletSpeed;
        Destroy(bullet, destroyTime);
        
        yield return new WaitForSeconds(shootTime);
        
        _canShoot = true;
        playerHud.UpdateAmmo((float)ammo / _maxAmmo);
        _playerAnimator.SetBool("IsShooting", false);
    }

    private void InputHandler()
    {
        if (Input.GetMouseButtonDown(1)) {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            mousePosition.z = 0;
            _canMove = true;
            _targetPosition = mousePosition;
            _isHoldingMove = true;
            if (multipleClickMultiplier < 1) {
                multipleClickMultiplier = 1;
            } else if (multipleClickMultiplier < maxMultipleClickMultiplier) {
                multipleClickMultiplier += 0.5f;multipleClickMultiplier += 0.5f;
            }
        } else if (Input.GetMouseButtonUp(1)) {
            _isHoldingMove = false;
        }

        if (Input.GetMouseButtonDown(0)) {
            Debug.Log("Can Shoot: " + _canShoot + " | Ammo: " + ammo + " | Health: " + health);
            if (!_canShoot || ammo <= 0 || health <= 0) return;
            StartCoroutine(Shoot());
            /*
            var rayHit = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(Input.mousePosition));
            
            if (!rayHit.collider) return;
            if (rayHit.collider.CompareTag("Interactable")) {
                
            } else {
                
            }
            */
        }

        if (Input.GetMouseButtonDown(2)) {
            
        }
    }
    
    private void HighlightInteractable()
    {
        var rayHit = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(Input.mousePosition));
        if (!rayHit.collider) return;
        
        if (rayHit.collider.CompareTag("Interactable")) {
            //rayHit.collider.GetComponent<SpriteRenderer>().color = Color.red;
        } else {
            //Debug.Log("Hovered over something else.");
        }
    }

    private void PlayerActions()
    {
        if (_canMove) {
            _playerAnimator.SetBool("IsRunning", true);
            if (_isHoldingMove) {
                _targetPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                _targetPosition.z = 0;
            }
            player.transform.position = Vector3.MoveTowards(player.transform.position, _targetPosition, speed * Time.deltaTime);
            FlipPlayer();
            if (player.transform.position == _targetPosition && !_isHoldingMove) {
                _canMove = false;
                multipleClickMultiplier = 0;
            }
        } else {
            _playerAnimator.SetBool("IsRunning", false);
        }
    }
    
    void Start()
    {
        _playerAnimator = player.GetComponent<Animator>();
        _playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
        ammo = _maxAmmo;
        health = _maxHealth;
        playerHud.UpdateAmmo((float)ammo / _maxAmmo);
        playerHud.UpdateHealth(health / _maxHealth);
    }
    
    void Update()
    {
        HighlightInteractable();
        InputHandler();
        PlayerActions();
        FlipPlayer();
        GunRotation();
        
        if (ammo <= 0) {
            _playerAnimator.SetBool("IsShooting", false);
            StartCoroutine(RefillAmmo());
        }
        if (health <= 0) {
            _playerAnimator.SetBool("IsDead", true);
        }
    }
}
