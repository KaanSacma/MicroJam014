using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSystem : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float multipleClickMultiplier = 0;
    [SerializeField] private float maxMultipleClickMultiplier = 2.0f;
    [SerializeField] public GameObject grid;
    [SerializeField] public GameObject roomTemplates;
    [SerializeField] public GameObject EntryRoom;
    
    [Header("Player")]
    [SerializeField] private GameObject player;
    [SerializeField] private Vector3 playerSpawnPosition;
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
    [SerializeField] private float shootCooldown = 0.5f;
    
    
    [Header("Bullet")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float destroyTime = 5.0f;
    [SerializeField] public float bulletDamage = 0.5f;
    
    private Animator _playerAnimator;
    private SpriteRenderer _playerSpriteRenderer;
    private Vector3 _targetPosition;
    private bool _canMove = false;
    private bool _isHoldingMove = false;
    private bool _canShoot = true;
    private int _maxAmmo = 3;
    private float _maxHealth = 3f;
    private float _currentShootCooldown = 0.0f;
    private float _currentRefillTime = 0.0f;
    private AnimationClip[] _playerAnimationClips;
    
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
        _canShoot = false;
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
        _playerAnimator.SetBool("IsShooting", true);
        player.GetComponent<AudioSource>().Play();
   
        
        yield return new WaitForSeconds(shootTime);
        
        _playerAnimator.SetBool("IsShooting", false);
        
        GameObject bullet = Instantiate(bulletPrefab, playerGun.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().velocity = direction.normalized * bulletSpeed;
        Destroy(bullet, destroyTime);
        
        ammo--;
        playerHud.UpdateAmmo((float)ammo / _maxAmmo);
        _canShoot = true;
    }
    
    private IEnumerator SpawnBackPlayer()
    {
        player.transform.position = playerSpawnPosition;
        health = _maxHealth;
        ammo = _maxAmmo;
        playerHud.UpdateAmmo((float)ammo / _maxAmmo);
        playerHud.UpdateHealth(health / _maxHealth);
        _playerAnimator.SetBool("IsDead", false);
        
        yield return new WaitForSeconds(0.1f);
        
        for (int i = 0; i < grid.transform.childCount; i++) {
            if (grid.transform.GetChild(i).name.Contains("(Clone)") || grid.transform.GetChild(i).name.Contains("Entry Room")) {
                Destroy(grid.transform.GetChild(i).gameObject);
            }
        }
        roomTemplates.GetComponent<RoomTemplates>().spawnedBoss = false;
        roomTemplates.GetComponent<RoomTemplates>().waitTime = roomTemplates.GetComponent<RoomTemplates>().maxWaitTime;
        roomTemplates.GetComponent<RoomTemplates>().rooms.Clear();
        
        GameObject entry = Instantiate(EntryRoom, new Vector3(-3, 100, 0), Quaternion.identity);
        entry.transform.parent = grid.transform;
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
            _playerAnimator.SetBool("IsShooting", false);
            if (!_canShoot || ammo <= 0 || health <= 0 && _currentShootCooldown >= shootCooldown) return;
            _currentShootCooldown = 0.0f;
            StartCoroutine(Shoot());
        }
        
        if (Input.GetKeyDown(KeyCode.Escape)) {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
        }
    }

    private void PlayerActions()
    {
        if (_canMove && health > 0) {
            var playerPos = mainCamera.WorldToScreenPoint(gameObject.transform.position);
            if (Screen.safeArea.Contains(playerPos) == false) {
                _canMove = false;
                _isHoldingMove = false;
                _playerAnimator.SetBool("IsRunning", false);
                return;
            }
            if (_isHoldingMove) {
                _targetPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                _targetPosition.z = 0;
            }
            _playerAnimator.SetBool("IsRunning", true);
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
        _playerAnimationClips = _playerAnimator.runtimeAnimatorController.animationClips;
    }
    
    void Update()
    {
        _currentShootCooldown += Time.deltaTime;
        _currentRefillTime += Time.deltaTime;

        if (player.GetComponent<Rigidbody2D>().velocity.magnitude > 0) {
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }

        InputHandler();
        PlayerActions();
        FlipPlayer();
        GunRotation();
        
        if (ammo <= 0 && health > 0 && _currentRefillTime >= refillTime) {
            _currentRefillTime = 0.0f;
            _canShoot = false;
            StartCoroutine(RefillAmmo());
        }
        if (health <= 0 && _playerAnimator.GetBool("IsDead") == false) {
            _playerAnimator.SetBool("IsDead", true);
            StartCoroutine(SpawnBackPlayer());
        }
    }
    
    public IEnumerator TakeDamage(float damage)
    {
        health -= damage;
        playerHud.UpdateHealth(health / _maxHealth);
        player.GetComponent<SpriteRenderer>().color = new Color(0.69f, 0.24f, 0.32f, 1);
        yield return new WaitForSeconds(0.1f);
        player.GetComponent<SpriteRenderer>().color = Color.white;
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Bullet")) return;
        if (other.gameObject.CompareTag("EnemyBullet")) {
            Destroy(other.gameObject);
            StartCoroutine(TakeDamage(other.gameObject.GetComponent<Bullet>().damage));
        } else if (other.gameObject.CompareTag("Ennemy")) {
            StartCoroutine(TakeDamage(other.gameObject.GetComponent<SimpleEnemy>().gunDamage));
        }
    }
}
