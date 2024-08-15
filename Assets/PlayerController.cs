using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("++Components++")]
    //Components
    public Rigidbody2D rbComponent;
    [SerializeField] Camera mainCamera;
    [SerializeField] SpriteRenderer spriteComponent;

    [Header("++Movement++")]
    [Header("Horizontal Movement")]
    public float maxSpeed = 5.0f;
    public float airAccel = 3.0f;
    public float maxAirSpeed = 7.0f;
    public float acceleration = 5.0f;
    public float deceleration = 5.0f;

    [Header("Jumping")]
    public float jumpSpeed = 8.0f;
    public float flightSpeed = 3.0f;
    public float flightTime = 1.0f;
    public float flightTimer = 0.0f;

    [Header("Dashing")]
    public float dashSpeed = 9.0f;
    public float dashTime = 0.5f;
    public float dashIntervalTime = 0.7f;
    bool isDashInterval = false;
    float dashTimer = 0.0f;
    float dashIntervalTimer = 0.0f;
    public bool isDashing = false;
    int dashTapCount = 0;
    public float dashCooldown = 1.0f;
    public bool isDashCooldown = false;
    float dashCooldownTimer = 0.0f;

    enum movementStates { idle, walk, run, fly, fall, jump }

    [Header("Vectors")]
    public Vector2 movementInput;

    [Header("Ground Check")]
    [SerializeField] GroundCheck groundCheckComponent;
    bool isGrounded = false;
    public BoxCollider2D currentPlatform;
    public BoxCollider2D colliderComponent;
    public float maxDistnceFromPlatform = 0.0f;

    [Header("++Combat++")]
    [Header("Health")]
    public int health = 5;
    public int baseHealth = 5;
    public bool isInvincible = false;
    public float invincibleDuration = 1.0f;
    Coroutine invincibleCoroutine;

    [Header("Magic")]
    public float MP = 20.0f;
    public float maxMP = 20.0f;
    public float magicCost = 2.0f;
    public float magicDamage = 10.0f;
    public float magicCharge = 1.0f;
    public float magicProjSpeed = 25.0f;
    public float magicProjDuration = 5.0f;
    [SerializeField] GameObject projectilePrefab;

    [Header("Melee")]
    public float meleeInterval = 0.3f;
    public float meleeDamage = 100.0f;
    public float meleeRange = 7.0f;
    Coroutine meleeCoroutine;


    void Start()
    {
        health = baseHealth;
        MP = maxMP;
        Vector2 colliderSize = colliderComponent.size / 2.0f;
        maxDistnceFromPlatform = Mathf.Ceil(colliderSize.magnitude);
        if(mainCamera == null) { 
            mainCamera = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        MoveInput();
        Dashing();
        AttackInput();
    }

    void MoveInput() {
        //Moving horizontally
        movementInput.x = Input.GetAxisRaw("Horizontal");

        if (isGrounded && !isDashing) {
            flightTimer = 0.0f; //Reset flight time
            rbComponent.AddForce(movementInput * acceleration);
            //Capping the horizontal speed
            if (rbComponent.velocity.magnitude >= maxSpeed) {
                Vector2 velocityDir = rbComponent.velocity.normalized;
                rbComponent.velocity = new Vector2(velocityDir.x * maxSpeed, rbComponent.velocity.y);
            }

            //Horizontal deceleration
            if (movementInput.x == 0) {
                rbComponent.velocity -= 0.1f * deceleration * rbComponent.velocity.normalized * Vector2.right;
            }
        }

        //If not grounded use the air speed
        else {
            rbComponent.AddForce(movementInput * airAccel);

            if (rbComponent.velocity.magnitude >= maxAirSpeed && !isDashing) {
                Vector2 velocityDir = rbComponent.velocity.normalized;
                rbComponent.velocity = new Vector2(velocityDir.x * maxAirSpeed, rbComponent.velocity.y);
            }
        }

        //Moving down from a platform
        if(Input.GetAxisRaw("Vertical") <= -1.0f) {
            IgnorePlatformCollision();
        }
        //Restoring the collisions if the player moves a certain distance from the platform
        if(currentPlatform != null) {
            Vector2 distance = currentPlatform.transform.position - transform.position;
            Debug.DrawLine(transform.position, new Vector3(transform.position.x, currentPlatform.transform.position.y, 0.0f), Color.red);
            float distanceToPlatform = Mathf.Abs(distance.y);
            //Debug.Log(distanceToPlatform);

            if(distanceToPlatform > maxDistnceFromPlatform) {
                Physics2D.IgnoreCollision(currentPlatform, GetComponent<BoxCollider2D>(), false);
                currentPlatform = null;
            }
        }

        //Jumping
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (isGrounded) {
                rbComponent.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
            }
        }

        //Flying
        if (Input.GetKey(KeyCode.Space)) {
            if (!isGrounded) {
                if (flightTimer <= flightTime) {
                    flightTimer += Time.deltaTime;
                    rbComponent.AddForce(Vector2.up * flightSpeed, ForceMode2D.Impulse);
                } 
            }
        }
    }

    void Dashing() {
        if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)) {
            if (!isDashCooldown && !isDashing) {
                isDashInterval = true;
                dashTapCount++;
            }
        }

        if (dashTapCount >= 2) {
            isDashing = true;
            dashTapCount = 0;
        }

        if (isDashInterval) {
            dashIntervalTimer += Time.deltaTime;
            if(dashIntervalTimer >= dashIntervalTime) {
                isDashInterval = false;
                dashIntervalTimer = 0.0f;
                dashTapCount = 0;
            }
        }

        if (isDashing) {
            rbComponent.AddForce(movementInput * dashSpeed, ForceMode2D.Impulse);
            dashTimer += Time.deltaTime;
            if(dashTimer >= dashTime) {
                isDashCooldown = true;
                isDashing = false;
                dashTimer = 0.0f;

            }
        }

        if (isDashCooldown) {
            dashCooldownTimer += Time.deltaTime;
            if(dashCooldownTimer >= dashCooldown) {
                isDashCooldown = false;
                dashCooldownTimer = 0.0f;
            }
        }
    }

    void GroundCheck() {
        isGrounded = groundCheckComponent.isGrounded;
    }

    void IgnorePlatformCollision() {
        if(currentPlatform != null) {
            Physics2D.IgnoreCollision(currentPlatform, GetComponent<BoxCollider2D>());
        }
    }

    void AttackInput() {
        MP += Time.deltaTime;
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 playerPos = transform.position;
        Vector2 direction = (mousePos - playerPos).normalized;

        //Melee attack
        if (Input.GetMouseButtonUp(0)) {
            if(meleeCoroutine == null) {
                meleeCoroutine = StartCoroutine(MeleeCoroutine(direction));
            }
        }

        //Magic attack
        if(Input.GetMouseButtonUp(1)) {
            Magic(direction);
        }
    }
    IEnumerator MeleeCoroutine(Vector2 direction) {
        yield return new WaitForSeconds(meleeInterval);
        Melee(direction);
        meleeCoroutine = null;

    }
    void Melee(Vector2 direction) {
        Debug.Log(direction);
        Debug.DrawRay(transform.position, direction * meleeRange, Color.red, 3.0f);
        RaycastHit2D[] hit =  Physics2D.RaycastAll(transform.position, direction, meleeRange);
        foreach(RaycastHit2D obj in hit) {
            if(obj.collider.gameObject.tag == "Boss") {
                obj.collider.gameObject.GetComponent<BossBehaviour>().TakeDamage(meleeDamage);
                Debug.Log("Hit boss");
            }
        }
    }

    void Magic(Vector2 direction) { 
        if(MP < magicCost) { return; }
        MP -= magicCost;
        PlayerProjectile proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity).GetComponent<PlayerProjectile>();
        proj.Set(direction, magicDamage, magicProjDuration, magicProjSpeed);
    }

    public void TakeDamage(int amount) {
        if (isInvincible) { return; }
        health -= amount;
        if(health <= 0 ) {
            //Death
        }
        isInvincible = true;
        if(invincibleCoroutine == null) {
            invincibleCoroutine = StartCoroutine(IFrame());
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if(collision.gameObject.tag == "Platform") {
            currentPlatform = collision.gameObject.GetComponent<BoxCollider2D>();
        }
    }

    IEnumerator IFrame() {
        spriteComponent.color = Color.red;
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
        spriteComponent.color = Color.white;
        invincibleCoroutine = null;
    }
}
