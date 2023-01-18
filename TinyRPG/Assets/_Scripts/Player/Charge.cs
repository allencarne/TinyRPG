using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge : MonoBehaviour
{
    [SerializeField] GameObject hitSpark;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            Instantiate(hitSpark, collision.transform.position, collision.transform.rotation);

            // Get Enemy Components
            var enemy = collision.gameObject.GetComponent<Enemy>();
            var enemyRB = collision.gameObject.GetComponent<Rigidbody2D>();

            // Triggers Enemy Hit State
            //enemy.enemyHit = true;

            // Stun Enemy
            enemy.enemyStunnedTrigger = true;

            // Knockback
            Vector2 direction = (enemy.transform.position - transform.position).normalized;
            enemyRB.velocity = direction * Player.dashKnockBackForce;
        }

        if (collision.tag == "Dummy")
        {
            Instantiate(hitSpark, collision.transform.position, collision.transform.rotation);

            // Get Enemy Components
            var enemy = collision.gameObject.GetComponent<TrainingDummy>();
            var enemyRB = collision.gameObject.GetComponent<Rigidbody2D>();

            // Triggers Enemy Hit State
            //enemy.enemyHit = true;

            // Stun Enemy
            enemy.dummyStunned = true;

            // Knockback
            Vector2 direction = (enemy.transform.position - transform.position).normalized;
            enemyRB.velocity = direction * Player.dashKnockBackForce;
        }
    }
}
