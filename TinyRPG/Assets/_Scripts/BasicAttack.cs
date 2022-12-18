using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : MonoBehaviour
{
    public void AE_BasicAttack2()
    {
        Player.canBasicAttack2 = true;
        Destroy(gameObject);
    }
}
