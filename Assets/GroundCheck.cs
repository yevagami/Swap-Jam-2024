using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public bool isGrounded = false;
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Platform") {
            isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Platform") {
            isGrounded = false;
        }
    }
}
