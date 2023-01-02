using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashForceField : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            Player.dashCollide = true;
        }
    }
}
