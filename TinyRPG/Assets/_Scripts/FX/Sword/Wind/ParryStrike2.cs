using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryStrike2 : MonoBehaviour
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
            var enemy = collision.gameObject.GetComponent<Enemy>();
            var enemyRB = collision.gameObject.GetComponent<Rigidbody2D>();

            Instantiate(hitSpark, collision.transform.position, collision.transform.rotation);

            // Deal Damage
            enemy.EnemyHurtState(Player.windSlashDamage);

            // Stun
            enemy.enemyStunnedTrigger = true;
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
