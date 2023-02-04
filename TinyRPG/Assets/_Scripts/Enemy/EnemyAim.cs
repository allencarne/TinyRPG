using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAim : MonoBehaviour
{
    public float offset;
    public Transform firePoint;
    Transform target;
    public static bool pauseDirection = false;

    private void Awake()
    {
        target = GameObject.Find("Aim").transform;
    }

    void Update()
    {
        if (!pauseDirection)
        {
            Rotate();
        }
    }

    public void Rotate()
    {
        Vector3 difference = target.position - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + offset);
    }
}
