using UnityEngine;
using System.Collections.Generic;

public class ShockwaveSegment : MonoBehaviour
{
    private static HashSet<Collider2D> hitSet = new HashSet<Collider2D>();

    [SerializeField] private float launchForce = 20f;
    [SerializeField] private LayerMask whatToHit;

    private void Start()
    {
        // 用BoxCollider2D的尺寸和位置做一次主动检测
        var box = GetComponent<BoxCollider2D>();
        if (box == null) return;

        Vector2 center = (Vector2)transform.position + box.offset;
        Vector2 size = box.size;
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f, whatToHit);

        foreach (var other in hits)
        {
            if (hitSet.Contains(other)) continue;

            if (other.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                rb.AddForce(Vector2.up * launchForce, ForceMode2D.Impulse);
                hitSet.Add(other);
            }

            if (other.TryGetComponent<FragileWall>(out FragileWall wall))
            {
                wall.Break();
                hitSet.Add(other);
            }
        }

        AudioManager.Instance.PlaySfx(AudioManager.Instance.shockwaveLaunch);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & whatToHit) == 0) return;
        if (hitSet.Contains(other)) return;

        if (other.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.AddForce(Vector2.up * launchForce, ForceMode2D.Impulse);
            hitSet.Add(other);
        }
    }

    public static void ResetHitSet()
    {
        hitSet.Clear();
    }

    public void OnFinished()
    {
        Destroy(gameObject);
    }
}