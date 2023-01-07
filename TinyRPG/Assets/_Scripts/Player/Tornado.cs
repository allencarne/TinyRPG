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

            var enemy = collision.gameObject.GetComponent<TrainingDummy>();

            var enemyRB = collision.gameObject.GetComponent<Rigidbody2D>();

            enemy.enemyHit = true;

            Vector2 direction = (enemy.transform.position - playerTransform.position).normalized;

            enemyRB.velocity = direction * -12;
        }
    }
}
