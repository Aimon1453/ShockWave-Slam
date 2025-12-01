using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    [Header("要生成的敌人Prefab")]
    public GameObject enemyPrefab;

    [Header("生成点（为空则用自身位置）")]
    public Transform spawnPoint;

    [Header("检测的敌人实例")]
    public GameObject currentEnemy;

    [Header("生成延迟（秒）")]
    public float respawnDelay = 1f;

    private void Start()
    {
        if (currentEnemy == null)
        {
            SpawnEnemy();
        }
    }

    private void Update()
    {
        // 检查敌人是否被销毁
        if (currentEnemy == null)
        {
            StartCoroutine(RespawnCoroutine());
        }
    }

    private IEnumerator RespawnCoroutine()
    {
        // 防止多次协程
        yield return new WaitForSeconds(respawnDelay);
        if (currentEnemy == null)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        Vector3 pos = spawnPoint ? spawnPoint.position : transform.position;
        currentEnemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
    }
}