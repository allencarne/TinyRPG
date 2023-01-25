using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBasicAttack : MonoBehaviour
{
    [SerializeField] GameObject hitSpark;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")  
        {
            // Components
            var player = collision.gameObject.GetComponent<Player>();
            var playerRB = collision.gameObject.GetComponent<Rigidbody2D>();

            // HitSpark
            Instantiate(hitSpark, collision.transform.position, collision.transform.rotation);

            // Deal Damage
            player.PlayerHurtState(1);

            // KnockBack
            Vector2 direction = (player.transform.position - transform.position).normalized;
            playerRB.velocity = direction * Enemy.basicAttackDamage;
        }
    }
}
