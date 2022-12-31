using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : MonoBehaviour
{
    [SerializeField] GameObject hitSpark;
    [SerializeField] float knockBackForce;
    //public static bool enemyHit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            Instantiate(hitSpark, collision.transform.position, collision.transform.rotation);

            var enemy = collision.gameObject.GetComponent<TrainingDummy>();

            var enemyRB = collision.gameObject.GetComponent<Rigidbody2D>();

            enemy.enemyHit = true;

            Vector2 direction = (transform.position + enemy.transform.position).normalized;

            enemyRB.velocity = direction * knockBackForce;
        }
    }
}
