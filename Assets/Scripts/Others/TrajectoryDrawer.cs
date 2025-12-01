using UnityEngine;
using System.Collections.Generic;

public class TrajectoryDrawer
{
    private List<GameObject> trajectoryPoints = new List<GameObject>(); // 路径点对象池
    private GameObject pointPrefab; // 路径点预制体
    private int pointCount; // 路径点数量
    private float timeStep; // 时间步长

    public TrajectoryDrawer(GameObject pointPrefab, int pointCount, Transform parent, float timeStep = 0.1f)
    {
        this.pointPrefab = pointPrefab;
        this.pointCount = pointCount;
        this.timeStep = timeStep;

        // 初始化路径点对象池
        for (int i = 0; i < pointCount; i++)
        {
            GameObject point = Object.Instantiate(pointPrefab, parent);
            point.SetActive(false);
            trajectoryPoints.Add(point);
        }
    }

    // 更新路径点位置
    public void UpdateTrajectory(Vector2 startPosition, Vector2 initialVelocity, float gravityScale)
    {
        float gravity = Physics2D.gravity.y * gravityScale; // 重力加速度

        for (int i = 0; i < trajectoryPoints.Count; i++)
        {
            float t = i * timeStep; // 当前时间
            float x = startPosition.x + initialVelocity.x * t;
            float y = startPosition.y + initialVelocity.y * t + 0.5f * gravity * t * t;

            Vector2 pointPosition = new Vector2(x, y);
            trajectoryPoints[i].SetActive(true);
            trajectoryPoints[i].transform.position = pointPosition;
        }
    }

    // 清除路径点
    public void ClearTrajectory()
    {
        foreach (var point in trajectoryPoints)
        {
            point.SetActive(false);
        }
    }
}