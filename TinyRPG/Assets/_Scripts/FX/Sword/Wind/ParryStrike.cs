using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryStrike : MonoBehaviour
{
    Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            player.parryStrikeTrigger = true;
        }

        if (collision.tag == "Dummy")
        {
            player.parryStrikeTrigger = true;
        }
    }
}
