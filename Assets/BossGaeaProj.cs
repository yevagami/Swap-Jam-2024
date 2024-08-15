using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGaeaProj : MonoBehaviour
{
    [SerializeField] Rigidbody2D rbComponent;
    public float duration;
    public float expandSpeed;
    public int damage;


    void Update() {
        StartCoroutine(projectileDuration());
        gameObject.transform.localScale += new Vector3(1, 1, 1) * expandSpeed * Time.deltaTime; 
    }

    IEnumerator projectileDuration() {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    public void Set(float es_, int dmg_, float dur_ = 5.0f) {
        expandSpeed = es_;
        damage = dmg_;
        duration = dur_;
    }
}
