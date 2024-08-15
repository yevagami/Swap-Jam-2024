using System.Collections;
using UnityEngine;

public class BossBehaviour : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Rigidbody2D rbComponent;

    [Header("Health")]
    public float baseHealth = 100.0f;
    public float currentHealth;

    [Header("Player Ref")]
    public GameObject playerObject;
    public PlayerController player;
    public Vector3 idleOffset = Vector3.zero;
    Vector3 dampVel;
    public float dampTime;
    public float idleTime = 1.0f;
    public int currentAction = 0;

    enum states { idle, act, die }
    states currentState = states.idle;
    Coroutine actionCoroutine = null; 
    Coroutine actionDuration = null;

    [Header("Myriad Arrows")]
    public float myriadArrowsDuration = 5.0f;
    public float myriadArrowsInterval = 0.5f;
    public int maProjDamage = 1;
    public float maProjDuration = 1.0f;
    public float maProjSpeed = 50.0f;
    public GameObject maProj;

    [Header("Hassou Tobi")]
    public float angleSpeed = 1.0f;
    public float hassouTobiDuration = 3.0f;
    public float hassoutTobiShootInterval = 0.0f;
    public int HTProjDamage = 1;
    public float HTProjDuration = 3.0f;
    public float HTProjSpeed = 10.0f;
    float angle = 0.0f;
    public GameObject HTProj;

    [Header("Megidoloan")]
    public float megidoDuration = 5.0f;
    public float megidoShootInterval = 0.0f;
    public int mgdProjDamage = 3;
    public float mgdProjSpeed = 5.0f;
    public float mgdProjDuration = 10.0f;
    public GameObject megidoProj;

    [Header("Gaea Rage")]
    public float gaeaRageDropInterval = 0.3f;
    public int grProjDamage = 10;
    public float grProjDuration = 7.0f;
    public float grProjSpeed = 10.0f;
    public GameObject grProj;

    [Header("Lunge")]
    public bool isLunging = false;
    public float lungeDuration = 7.0f;
    public float lungeSpeed = 10.0f;

    [Header("Freikugel")]
    public float freikugelChargeTime = 1.3f;
    public float frkglDuraion = 1.0f;
    public int frkglDamage = 10;
    public float frkglActivation = 0.3f;
    public GameObject freikugelProj;

    [Header("Riot Gun")]
    public float rgInterval = 0.5f;
    public float rgProjectileOffset = 2.0f;
    public float rgProjectileSpeed = 20.0f;
    public float rgProjectileDuration = 1.5f;
    public int rgProjectileCount = 9; //Must be an odd number
    public float rgStartDistanceFromPlyr = 7.0f;
    public float rgDuration = 5.0f;
    public int rgDamage = 5;
    public GameObject rgProjPrefab;

    [Header("Mediarahan")]
    public float mdrhnDelay = 1.0f;
    public float mdrhnHealPercentage = 10.0f;

    void Start()
    {
        currentHealth = baseHealth;        
    }

    // Update is called once per frame
    void Update()
    {
        switch(currentState) {

            //By default it will follow the player
            case states.idle:
                transform.position = Vector3.SmoothDamp(transform.position, playerObject.transform.position + idleOffset, ref dampVel, dampTime);
                if(actionCoroutine == null) {
                    actionCoroutine = StartCoroutine(Idle());
                }
                break;

            case states.act:
                switch (currentAction) {
                    case 0:
                        MyriadArrows();
                        break;

                    case 1:
                        HassouTobi();
                        break;

                    case 2:
                        Megidoloan();
                        break;

                    case 3:
                        GaeaRage();
                        break;

                    case 4:
                        Lunge();
                        break;

                    case 5:
                        Freikugel();
                        break;

                    case 6:
                        RiotGun();
                        break;

                    case 7:
                        Mediarahan();
                        break;
                }

                break;
        }
    }

    public void TakeDamage(float damage) { 
        currentHealth -= damage;
        if(currentHealth <= 0) {
            Debug.Log("BOSS DEAD");
        }
    }

    IEnumerator Idle() {
        yield return new WaitForSeconds(idleTime);
        currentAction = Random.Range(0,7);
        currentState = states.act;
        actionCoroutine = null;
    }


    //Myriad arrows
    //Every 0.5 seconds fire a projectile towards the player
    //Lasts for 5 seconds
    void MyriadArrows() {
        transform.position = Vector3.SmoothDamp(transform.position, playerObject.transform.position + idleOffset, ref dampVel, dampTime);
        Debug.DrawLine(transform.position, playerObject.transform.position, Color.cyan);

        if(actionCoroutine == null) {
            actionCoroutine = StartCoroutine(ShootMyriadArrows());
        }

        if(actionDuration == null) {
            actionDuration = StartCoroutine(Duration(myriadArrowsDuration));
        }
    }
    IEnumerator ShootMyriadArrows() {
        yield return new WaitForSeconds(myriadArrowsInterval);
        Vector2 playerDir = (playerObject.transform.position - transform.position).normalized;
        BossProjectile proj = Instantiate(maProj, transform.position, Quaternion.identity).GetComponent<BossProjectile>();
        proj.Set(playerDir, maProjDamage, maProjDuration, maProjSpeed);
        actionCoroutine = null;
    }
    IEnumerator Duration(float time) {
        yield return new WaitForSeconds(time);
        currentState = states.idle;
        actionDuration = null;
    }

    

    //HassouTobi
    //Fires projectiles in a windmill pattern
    void HassouTobi() {
        transform.position = Vector3.SmoothDamp(transform.position, playerObject.transform.position + idleOffset, ref dampVel, 0.1f);
        angle += Time.deltaTime * angleSpeed;
        Debug.DrawRay(transform.position, new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0), Color.cyan);
        Debug.DrawRay(transform.position, new Vector3(-Mathf.Sin(angle), Mathf.Cos(angle), 0), Color.cyan);
        Debug.DrawRay(transform.position, new Vector3(Mathf.Sin(angle), -Mathf.Cos(angle), 0), Color.cyan);
        Debug.DrawRay(transform.position, new Vector3(-Mathf.Cos(angle), -Mathf.Sin(angle), 0), Color.cyan);
        
        if(actionCoroutine == null) {
            actionCoroutine = StartCoroutine(ShootHassouTobi());
        }

        if(actionDuration == null) {
            actionDuration = StartCoroutine(Duration(hassouTobiDuration));
        }
    }
    IEnumerator ShootHassouTobi() {
        yield return new WaitForSeconds(hassoutTobiShootInterval);
        Vector2[] projDirections = new Vector2[4];
        projDirections[0] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        projDirections[1] = new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle));
        projDirections[2] = new Vector2(Mathf.Sin(angle), -Mathf.Cos(angle));
        projDirections[3] = new Vector2(-Mathf.Cos(angle), -Mathf.Sin(angle));

        for(int i = 0; i < projDirections.Length; i++) {
            BossProjectile proj = Instantiate(HTProj, transform.position, Quaternion.identity).GetComponent<BossProjectile>();
            proj.Set(projDirections[i], HTProjDamage, HTProjDuration, HTProjSpeed);
        }
        actionCoroutine = null;
    }



    //Megidoloan
    //Fire a bunh of giant homing, slow moving projectiles
    void Megidoloan() {
        transform.position = Vector3.SmoothDamp(transform.position, playerObject.transform.position + idleOffset, ref dampVel, dampTime);

        if (actionCoroutine == null) {
            actionCoroutine = StartCoroutine(ShootMegidoloan());
        }

        if (actionDuration == null) {
            actionDuration = StartCoroutine(Duration(megidoDuration));
        }
    }
    IEnumerator ShootMegidoloan() {
        yield return new WaitForSeconds(megidoShootInterval);
        BossHomingProjectile proj = Instantiate(megidoProj, transform.position, Quaternion.identity).GetComponent<BossHomingProjectile>();
        proj.Set(playerObject, mgdProjDamage, mgdProjDuration, mgdProjDuration);
        actionCoroutine = null;
    }



    //Gaea Rage
    //Spawns a gaea rage projectile that quickly expands
    void GaeaRage() {
        transform.position = Vector3.SmoothDamp(transform.position, playerObject.transform.position + new Vector3(player.rbComponent.velocity.x * 2, 1,0), ref dampVel, 0.3f);
        if (actionCoroutine == null) {
            actionCoroutine = StartCoroutine(DropGaeaRage());
        }
    }
    IEnumerator DropGaeaRage() {
        yield return new WaitForSeconds(gaeaRageDropInterval);
        BossGaeaProj proj = Instantiate(grProj, transform.position, Quaternion.identity).GetComponent<BossGaeaProj>();
        proj.Set(10, 5, 7);

        actionCoroutine = null;
        actionDuration = null;
        currentState = states.idle;
    }



    //Mediarahan
    //Heal 10% of the boss's max HP
    void Mediarahan() {
        if(actionCoroutine == null) {
            actionCoroutine = StartCoroutine(Media());
        }
    }
    IEnumerator Media() {
        yield return new WaitForSeconds(mdrhnDelay);
        currentHealth += baseHealth * (mdrhnHealPercentage / 100.0f);
        actionCoroutine = null;
        currentState = states.idle;
    }



    //Lunge
    //Fly towards the player
    void Lunge() {
        Vector2 playerDir = playerObject.transform.position - transform.position;
        rbComponent.velocity = playerDir.normalized * lungeSpeed;

        if (actionDuration == null) {
            actionDuration = StartCoroutine(Duration(lungeDuration));
        }
    }



    //Freikugel
    //Charge and fire a beam towards the player
    void Freikugel() {
        if(actionCoroutine == null) {
            actionCoroutine = StartCoroutine(ChargeFreikugel());
        }
    }
    IEnumerator ChargeFreikugel() { 
        yield return new WaitForSeconds(freikugelChargeTime);
        BossBeam beam = Instantiate(freikugelProj,transform.position , Quaternion.identity).GetComponent<BossBeam>();
        
        Vector2 playerDir = playerObject.transform.position - transform.position;
        beam.Set(playerDir.magnitude * 4, playerDir.normalized, frkglDamage, frkglDuraion, frkglActivation);
        beam.transform.position = Vector3.Lerp(transform.position, player.transform.position, 0.5f);

        currentState = states.idle;
        actionCoroutine = null;
    }



    //Riot gun
    // fire a series of projectile walls
    void RiotGun() {
        transform.position = Vector3.SmoothDamp(transform.position, playerObject.transform.position + idleOffset, ref dampVel, dampTime);

        if (actionCoroutine == null) {
            actionCoroutine = StartCoroutine(RGShoot());
        }

        if(actionDuration == null) {
            actionDuration = StartCoroutine(Duration(rgDuration));
        }
    }
    IEnumerator RGShoot() {
        yield return new WaitForSeconds(rgInterval);
        int direction = Random.Range(0, 3); //0 - left, 1 - right, 2 - down
        int spacing = rgProjectileCount / 2;

        Vector3 startPos = Vector3.zero;
        Vector3 projDirection = Vector3.left;
        Vector3 startOffsetMultiplier = new Vector3(1, 0, 0);
        Vector3 offsetMultiplier = new Vector3(0, -1, 0);
        Vector3 spawnOffsetMultiplier = new Vector3(0, 1, 0);

        switch (direction) {
            case 0:
                projDirection = Vector3.left;
                startOffsetMultiplier = new Vector3(1, 0, 0);
                offsetMultiplier = new Vector3(0, -1, 0);
                spawnOffsetMultiplier = new Vector3(0, 1, 0);

                break;

            case 1:
                projDirection = Vector3.right;
                startOffsetMultiplier = new Vector3(-1, 0, 0);
                offsetMultiplier = new Vector3(0, -1, 0);
                spawnOffsetMultiplier = new Vector3(0, 1, 0);
                break;

            case 2:
                projDirection = Vector3.down;
                startOffsetMultiplier = new Vector3(0, 1, 0);
                offsetMultiplier = new Vector3(-1, 0, 0);
                spawnOffsetMultiplier = new Vector3(1, 0, 0);
                break;
        }

        startPos = playerObject.transform.position + (startOffsetMultiplier * rgStartDistanceFromPlyr);
        startPos += offsetMultiplier * spacing * rgProjectileOffset;
        for (int i = 0; i < rgProjectileCount; i++) {
            BossRiotGun proj = Instantiate(rgProjPrefab, startPos, Quaternion.identity).GetComponent<BossRiotGun>();
            proj.Set(projDirection, rgDamage, rgProjectileDuration, rgProjectileSpeed, startPos, startPos + projDirection * rgStartDistanceFromPlyr * 2);
            startPos += spawnOffsetMultiplier * rgProjectileOffset;
        }

        actionCoroutine = null;
    }
}
