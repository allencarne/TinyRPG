using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : MonoBehaviour
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
            Instantiate(hitSpark, collision.transform.position, collision.transform.rotation);
            Instantiate(hitSpark1, collision.transform.position, firePoint.transform.rotation);

            var enemy = collision.gameObject.GetComponent<TrainingDummy>();

            var enemyRB = collision.gameObject.GetComponent<Rigidbody2D>();

            enemy.enemyHit = true;

            Vector2 direction = (enemy.transform.position - transform.position).normalized;

            enemyRB.velocity = direction * Player.knockBackForce;
        }
    }
}
