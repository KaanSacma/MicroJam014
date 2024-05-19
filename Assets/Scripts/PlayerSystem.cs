using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerSystem : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private int ammo = 3;
    [SerializeField] private float health = 3f;
    [SerializeField] private GameObject player;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerHud playerHud;
    [SerializeField] private float multipleClickMultiplier = 0;
    [SerializeField] private float maxMultipleClickMultiplier = 2.5f;
    
    private Animator _playerAnimator;
    private SpriteRenderer _playerSpriteRenderer;
    private Vector3 _targetPosition;
    private bool _canMove = false;
    private bool _isHoldingMove = false;
    private bool _canShoot = true;
    private int _maxAmmo = 3;
    private float _maxHealth = 3f;
    
    private IEnumerator RefillAmmo()
    {
        _playerAnimator.SetBool("IsAmmoEmpty", true);
        
        yield return new WaitForSeconds(0.20f);
        
        _playerAnimator.SetBool("IsAmmoEmpty", false);
        ammo = _maxAmmo;
        playerHud.UpdateAmmo((float)ammo / _maxAmmo);
    }
    
    private IEnumerator Shoot()
    {
        Vector3 position = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = position - player.transform.position;
        
        _canShoot = false;
        if (direction.x < 0) {
            _playerSpriteRenderer.flipX = true;
        } else {
            _playerSpriteRenderer.flipX = false;
        }
        ammo--;
        _playerAnimator.SetBool("IsShooting", true);
        
        yield return new WaitForSeconds(0.15f);
        
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
            var rayHit = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(Input.mousePosition));
            
            if (!rayHit.collider) return;
            if (rayHit.collider.CompareTag("Interactable")) {
                
            } else {
                if (!_canShoot || ammo <= 0 || health <= 0) return;
                StartCoroutine(Shoot());
            }
            
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
            player.transform.position = Vector3.MoveTowards(player.transform.position, _targetPosition, (speed * Time.deltaTime) * multipleClickMultiplier);
            Vector2 direction = _targetPosition - player.transform.position;
            if (direction.x < 0) {
                _playerSpriteRenderer.flipX = true;
            } else {
                _playerSpriteRenderer.flipX = false;
            }
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
        
        if (ammo <= 0) {
            _playerAnimator.SetBool("IsShooting", false);
            StartCoroutine(RefillAmmo());
        }
        if (health <= 0) {
            _playerAnimator.SetBool("IsDead", true);
        }
    }
}
