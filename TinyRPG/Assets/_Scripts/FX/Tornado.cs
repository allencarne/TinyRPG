using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado : MonoBehaviour
{
    [SerializeField] GameObject hitSpark;
    Transform playerTransform;

    private void Awake()
    {
        playerTransform = GameObject.Find("Player").transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            Instantiate(hitSpark, collision.transform.position, collision.transform.rotation);

            var enemy = collision.gameObject.GetComponent<Enemy>();

            var enemyRB = collision.gameObject.GetComponent<Rigidbody2D>();

            // Deal Damage
            enemy.EnemyHurtState(Player.basicAttackDamage);

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