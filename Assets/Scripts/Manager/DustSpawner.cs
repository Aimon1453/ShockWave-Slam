using UnityEngine;

public class DustSpawner : MonoBehaviour
{
    [SerializeField] private GameObject dustPrefab;
    [SerializeField] private int maxDustCount = 10; // 屏幕上最多同时存在多少个
    [SerializeField] private float spawnInterval = 0.5f; // 生成间隔

    private float timer;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        // 简单的对象池逻辑：如果当前场景里的灰尘太少，就生成
        // 注意：这里为了简单直接用FindObjectsByType，如果性能要求高建议用List管理
        if (GameObject.FindGameObjectsWithTag("Dust").Length < maxDustCount)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                SpawnDust();
                timer = 0f;
            }
        }
    }

    private void SpawnDust()
    {
        // 获取屏幕内的随机位置 (Viewport 0,0 是左下角, 1,1 是右上角)
        Vector2 randomViewportPos = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));

        // 转换为世界坐标
        Vector2 spawnPos = cam.ViewportToWorldPoint(randomViewportPos);

        // 生成灰尘
        Instantiate(dustPrefab, spawnPos, Quaternion.identity);
    }
}