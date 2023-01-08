using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhirlWind : MonoBehaviour
{
    [SerializeField] Animator animator;
    Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        var angleToMouse = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        animator.SetFloat("Horizontal", angleToMouse.x);
        animator.SetFloat("Vertical", angleToMouse.y);
    }
}
