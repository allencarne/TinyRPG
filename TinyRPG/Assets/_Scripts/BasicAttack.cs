using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : MonoBehaviour
{
    [SerializeField] GameObject hitSpark;
    //public static bool enemyHit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            Instantiate(hitSpark, collision.transform.position, collision.transform.rotation);
            var enemy = collision.gameObject.GetComponent<TrainingDummy>();

            enemy.enemyHit = true;
            //enemyHit = true;
        }
    }
}
