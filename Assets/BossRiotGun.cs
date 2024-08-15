using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRiotGun : MonoBehaviour
{
    [SerializeField] Rigidbody2D rbComponent;
    [SerializeField] LineRenderer lineRenderer;
    public float moveSpeed;
    public float duration;
    public Vector2 direction;
    public int damage;

    void Update() {
        rbComponent.velocity = direction * moveSpeed;
        StartCoroutine(projectileDuration());
    }

    IEnumerator projectileDuration() {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    public void Set(Vector2 dir_, int dmg_, float dur_, float spd_, Vector2 start_, Vector2 end_) {
        direction = dir_;
        damage = dmg_;
        duration = dur_;
        moveSpeed = spd_;

        lineRenderer.positionCount = 2;
        Vector3[] vertices = new Vector3[2];
        vertices[0] = start_;
        vertices[1] = end_; 
        lineRenderer.SetPositions(vertices);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
