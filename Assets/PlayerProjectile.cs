using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [SerializeField] Rigidbody2D rbComponent;
    public float moveSpeed;
    public float duration;
    public Vector2 direction; 
    public float damage;


    void Update(){
        rbComponent.velocity = direction * moveSpeed;
        StartCoroutine(projectileDuration());
    }

    IEnumerator projectileDuration() {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    public void Set(Vector2 dir_, float dmg_, float dur_ = 5.0f, float spd_ = 10.0f) {
        direction = dir_;
        damage = dmg_;
        duration = dur_;
        moveSpeed = spd_;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "Boss") {
            BossBehaviour boss = collision.gameObject.GetComponent<BossBehaviour>();
            boss.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

}
