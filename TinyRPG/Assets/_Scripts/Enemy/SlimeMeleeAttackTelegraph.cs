using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeMeleeAttackTelegraph : MonoBehaviour
{
    private void Update()
    {
        if (Enemy.isEnemyHurt)
        {
            Destroy(gameObject);
        }
    }
}
