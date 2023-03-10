using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempestCharge2 : MonoBehaviour
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

            // Deal Damage
            enemy.EnemyHurtState(Player.windSlashDamage);

            // Stun Enemy
            enemy.enemyStunnedTrigger = true;

            // Knockback
            Vector2 direction = (enemy.transform.position - transform.position).normalized;
            enemyRB.velocity = direction * Player.windSlashKnockBackForce;
        }

        if (collision.tag == "Dummy")
        {
            Instantiate(hitSpark, collision.transform.position, collision.transform.rotation);

            var enemy = collision.gameObject.GetComponent<TrainingDummy>();

            var enemyRB = collision.gameObject.GetComponent<Rigidbody2D>();

            enemy.dummyHit = true;

            enemy.dummyStunned = true;
        }
    }
}
