using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public Transform roomCenter; // 拖入房间左上角锚点

    // 偏移量
    private Vector2 offset = new Vector2(15f, -8.55f);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 targetPos = roomCenter.position + (Vector3)offset;
            CameraManager.Instance.MoveToRoom(targetPos);
        }
    }
}