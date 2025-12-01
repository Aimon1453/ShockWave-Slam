using UnityEngine;
using System.Collections;

public class CannonTurret : MonoBehaviour
{
    [Header("设置")]
    [SerializeField] private GameObject projectilePrefab; // 拖入 HomingBullet 的 Prefab
    [SerializeField] private Transform spawnPoint;        // 炮口位置
    [SerializeField] private float reloadTime = 3f;       // 炮弹消失后的装填时间

    private GameObject currentProjectile;
    private bool isReloading = false;

    private void Start()
    {
        // 游戏开始时先发射一枚
        Fire();
    }

    private void Update()
    {
        // 检查当前炮弹是否存在
        // 如果 currentProjectile 变成了 null (被销毁了)，且没有在装填中
        if (currentProjectile == null && !isReloading)
        {
            StartCoroutine(ReloadAndFire());
        }
    }

    private IEnumerator ReloadAndFire()
    {
        isReloading = true;

        // 等待装填时间
        yield return new WaitForSeconds(reloadTime);

        Fire();

        isReloading = false;
    }

    private void Fire()
    {
        if (projectilePrefab != null && spawnPoint != null)
        {
            // 生成炮弹并记录引用
            currentProjectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);

            // 可选：播放发射音效/特效
        }
    }
}