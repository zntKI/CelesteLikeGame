using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolineBehaviour : MonoBehaviour
{
    [SerializeField] private float trampolineSpeed = 20;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !collision.collider.isTrigger)
        {
            var player = collision.gameObject;
            var rb = player.GetComponent<Rigidbody2D>();

            rb.velocity = Vector2.up * trampolineSpeed;
        }
    }
}
