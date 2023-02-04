using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    GameObject target;
    [SerializeField] float offset;

    private void Awake()
    {
        target = GameObject.Find("Player");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = new Vector3 (0, offset, 0);

        transform.position = target.transform.position + newPos;
    }
}
