using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempestCharge : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            Player.tempestChargeCollisionTrigger = true;
        }

        if (collision.tag == "Dummy")
        {
            Player.tempestChargeCollisionTrigger = true;
        }
    }
}
