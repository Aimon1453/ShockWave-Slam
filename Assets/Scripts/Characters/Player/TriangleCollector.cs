using UnityEngine;

public class TriangleCollector : MonoBehaviour
{
    [Header("UI 设置")]
    [Tooltip("按顺序拖入 UI 中的 Tri-1/Tri, Tri-2/Tri, Tri-3/Tri 物体")]
    [SerializeField] private GameObject[] triangleFills;

    private int currentTriangleCount = 0;

    private void Start()
    {
        // 游戏开始时，确保所有实心三角都是隐藏的
        foreach (var fill in triangleFills)
        {
            if (fill != null) fill.SetActive(false);
        }
    }

    public void CollectTriangle()
    {
        // 防止数组越界
        if (currentTriangleCount < triangleFills.Length)
        {
            // 激活对应的 UI 图标
            if (triangleFills[currentTriangleCount] != null)
            {
                triangleFills[currentTriangleCount].SetActive(true);
            }

            currentTriangleCount++;

            // 播放音效
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySfx(AudioManager.Instance.keyPickup);
            }

            if (currentTriangleCount >= triangleFills.Length)
            {
                // 获取 Player 组件并触发胜利 Pose
                GetComponent<Player>().TriggerVictoryPose();
            }

            if (currentTriangleCount >= triangleFills.Length)
            {
                var npc = FindFirstObjectByType<OldMan>();
                if (npc != null) npc.OnPlayerCollectAll();
            }
        }
    }
}