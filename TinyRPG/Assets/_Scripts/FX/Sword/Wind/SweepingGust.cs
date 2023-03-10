using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweepingGust : MonoBehaviour
{
    [SerializeField] GameObject hitSpark;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            // Components
            var enemy = collision.gameObject.GetComponent<Enemy>();
            var enemyRB = collision.gameObject.GetComponent<Rigidbody2D>();

            // HitSpark
            Instantiate(hitSpark, collision.transform.position, collision.transform.rotation);

            // Deal Damage
            enemy.EnemyHurtState(Player.windSlashDamage);

            // Slow
            enemy.enemySlowedTrigger = true;
        }

        if (collision.tag == "Dummy")
        {
            Instantiate(hitSpark, collision.transform.position, collision.transform.rotation);

            var enemy = collision.gameObject.GetComponent<TrainingDummy>();

            var enemyRB = collision.gameObject.GetComponent<Rigidbody2D>();

            enemy.dummyHit = true;

            enemy.dummySlowed = true;
        }
    }
}
