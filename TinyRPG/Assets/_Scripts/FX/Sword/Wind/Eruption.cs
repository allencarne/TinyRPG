using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eruption : MonoBehaviour
{
    [SerializeField] GameObject hitSpark;
    [SerializeField] GameObject hitSpark1;
    GameObject firePoint;

    private void Awake()
    {
        firePoint = GameObject.Find("Aim");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            // Components
            var enemy = collision.gameObject.GetComponent<Enemy>();
            var enemyRB = collision.gameObject.GetComponent<Rigidbody2D>();

            // Hit Spark
            Instantiate(hitSpark, collision.transform.position, collision.transform.rotation);
            Instantiate(hitSpark1, collision.transform.position, firePoint.transform.rotation);

            // Deal Damage
            enemy.EnemyHurtState(Player.windSlashDamage);

            // Knockback
            enemy.enemyKnockBackTrigger = true;
            Vector2 direction = (enemy.transform.position - transform.position).normalized;
            enemyRB.velocity = direction * Player.eruptionKnockBackForce;
        }

        if (collision.tag == "Dummy")
        {
            // Components
            var enemy = collision.gameObject.GetComponent<TrainingDummy>();
            var enemyRB = collision.gameObject.GetComponent<Rigidbody2D>();

            // Hit Spark
            Instantiate(hitSpark, collision.transform.position, collision.transform.rotation);
            Instantiate(hitSpark1, collision.transform.position, firePoint.transform.rotation);

            // Damage
            enemy.dummyHit = true;

            // Knockback
            Vector2 direction = (enemy.transform.position - transform.position).normalized;
            enemyRB.velocity = direction * Player.eruptionKnockBackForce;
        }
    }
}
