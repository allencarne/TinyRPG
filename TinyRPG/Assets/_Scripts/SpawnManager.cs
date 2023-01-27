using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Vector3 size;
    [SerializeField] Vector3 center;

    public static int enemyCount;
    public int maxEnemyCount;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (enemyCount < maxEnemyCount)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        enemyCount++;

        Vector3 pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2));

        Instantiate(enemyPrefab, pos, Quaternion.identity);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, size);
    }
}
