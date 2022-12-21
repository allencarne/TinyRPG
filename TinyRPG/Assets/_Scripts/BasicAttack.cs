using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : MonoBehaviour
{
    public static bool basicAttackTrigger;
    public static bool basicAttack2Trigger;

    public void AE_CanBasicAttack2()
    {
        //Player2.canBasicAttack2 = true;
    }

    public void AE_BasicAttackAnimationEnd()
    {
        //Player2.canBasicAttack2 = false;
        basicAttackTrigger = true;
    }

    public void AE_BasicAttack2AnimationEnd()
    {
        //basicAttack2Trigger = true;
    }
}
