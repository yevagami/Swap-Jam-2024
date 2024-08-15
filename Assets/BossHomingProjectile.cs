using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHomingProjectile : MonoBehaviour
{
    [SerializeField] Rigidbody2D rbComponent;
    public float moveSpeed;
    public float duration;
    public GameObject target;
    public int damage;


    void Update() {
        StartCoroutine(projectileDuration());
        if (target == null) { return; }
        Vector2 dir = target.transform.position - transform.position;
        rbComponent.velocity = dir.normalized * moveSpeed;
    }

    IEnumerator projectileDuration() {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    public void Set(GameObject t_, int dmg_, float dur_ = 5.0f, float spd_ = 10.0f) {
        target = t_;
        damage = dmg_;
        duration = dur_;
        moveSpeed = spd_;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
