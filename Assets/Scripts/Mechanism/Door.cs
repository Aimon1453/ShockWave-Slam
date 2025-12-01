using UnityEngine;
using PrimeTween;

public class Door : MonoBehaviour, IUnlockable
{
    [Header("锁ID")]
    [SerializeField] private int lockID = 0;

    private bool isOpened = false;

    public bool TryUnlock(int keyID)
    {
        if (isOpened) return false;

        // 检查钥匙ID是否匹配
        if (keyID == lockID)
        {
            OpenDoor();
            return true;
        }

        return false;
    }

    private void OpenDoor()
    {
        isOpened = true;
        AudioManager.Instance.PlaySfx(AudioManager.Instance.doorOpen); // 播放开门音效
        Vector3 targetPos = transform.position + Vector3.up * 2;

        Tween.Position(transform, endValue: targetPos, duration: 1, ease: Ease.OutQuad);
    }
}