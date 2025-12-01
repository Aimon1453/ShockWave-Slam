using UnityEngine;


public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite inactiveSprite;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        Deactivate();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                // 取消上一个检查点激活
                if (player.CurrentCheckpoint != null && player.CurrentCheckpoint != this)
                {
                    player.CurrentCheckpoint.Deactivate();
                }
                // 激活当前检查点
                player.CurrentCheckpoint = this;
                Activate();
            }
        }
    }

    public void Activate()
    {
        if (sr != null && activeSprite != null)
            sr.sprite = activeSprite;
        AudioManager.Instance.PlaySfx(AudioManager.Instance.checkPoint); // 播放激活检查点音效
    }

    public void Deactivate()
    {
        if (sr != null && inactiveSprite != null)
            sr.sprite = inactiveSprite;
    }

    public Vector3 GetRespawnPosition()
    {
        return transform.position;
    }
}