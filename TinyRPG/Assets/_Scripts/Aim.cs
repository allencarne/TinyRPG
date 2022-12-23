using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
    Player2 player2;
    public float offset;
    public Transform firePoint;
    [SerializeField] GameObject player;

    private void Awake()
    {
        player2 = player.GetComponent<Player2>();
    }

    void Update()
    {
        Rotate();
    }

    public void Rotate()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + offset);
    }
}
