using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryStrike : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        /*
        if (collision.tag == "Enemy")
        {
            Player.parryStrikeTrigger = true;
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
        */
        if (collision.tag == "EnemyAttack")
        {
            Player.parryStrikeTrigger = true;
            //Destroy(gameObject);
            //Destroy(collision.gameObject);
        }
        /*
        if (collision.tag == "Dummy")
        {
            Player.parryStrikeTrigger = true;
            Destroy(gameObject);
        }
        */
    }

    public void AE_ParryStrike()
    {
        Player.parryStrikeEnd = true;
    }
}
