using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : MonoBehaviour
{
    public static bool basicAttackTrigger;
    public static bool basicAttack2Trigger;
    public static bool basicAttack3Trigger;

    public void AE_CanBasicAttack2()
    {
        Player2.canBasicAttack2 = true;
    }

    public void AE_CanBasicAttack3()
    {
        Player2.canBasicAttack3 = true;
    }

    public void AE_BasicAttackAnimationEnd()
    {
        Player2.canBasicAttack2 = false;
        Player2.canBasicAttack3 = false;
        basicAttackTrigger = true;
    }

    public void AE_BasicAttack2AnimationEnd()
    {
        basicAttack2Trigger = true;
    }

    public void AE_BasicAttack3AnimationEnd()
    {
        basicAttack3Trigger = true;
    }
}
