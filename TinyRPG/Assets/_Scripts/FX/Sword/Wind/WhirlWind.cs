using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhirlWind : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] GameObject hitSpark;
    Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        var angleToMouse = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        animator.SetFloat("Horizontal", angleToMouse.x);
        animator.SetFloat("Vertical", angleToMouse.y);
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

            enemy.enemyStunnedTrigger = true;

            Vector2 direction = (enemy.transform.position - transform.position).normalized;

            enemyRB.velocity = direction * Player.basicAttackKnockBackForce;
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
