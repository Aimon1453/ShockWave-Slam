using UnityEngine;
using PrimeTween;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    private Tween cameraTween;

    private void Awake()
    {
        Instance = this;
    }

    public void MoveToRoom(Vector3 targetPos)
    {
        // 取消上一个Tween（如果有）
        if (cameraTween.isAlive)
            cameraTween.Stop();

        // 只移动XY，保持Z不变
        Vector3 startPos = transform.position;
        targetPos.z = startPos.z;

        cameraTween = Tween.Position(transform, startPos, targetPos, 0.5f, Ease.InOutSine);
    }
}