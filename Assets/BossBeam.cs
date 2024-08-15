using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class BossBeam : MonoBehaviour
{
    [SerializeField] Rigidbody2D rbComponent;
    public float length;
    public float duration;
    public Vector2 dir;
    public int damage;
    public bool activated = false;
    public float actiavtionTime = 0.1f;


    void Update() {
        StartCoroutine(projectileDuration());
        StartCoroutine(activationCoroutine());
        transform.localScale = new Vector3(length, transform.localScale.y, transform.localScale.z);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0,0, angle);
    }

    IEnumerator projectileDuration() {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
    IEnumerator activationCoroutine() {
        yield return new WaitForSeconds(actiavtionTime);
        activated = true;
    }

    public void Set(float l_, Vector2 dir_, int dmg_, float dur_ = 5.0f, float actTime = 0.3f) {
        length = l_;
        dir = dir_;
        damage = dmg_;
        duration = dur_;
        actiavtionTime = actTime;
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (!activated) { return; }
        if (collision.gameObject.tag == "Player") {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player.TakeDamage(damage);
        }
    }
}
