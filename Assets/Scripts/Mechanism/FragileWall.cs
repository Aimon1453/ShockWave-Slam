using System.Collections;
using UnityEngine;
using PrimeTween;

public class FragileWall : MonoBehaviour
{
    [SerializeField] private float disableTime = 3f;
    private Collider2D col;
    private SpriteRenderer sr;
    private Vector3 originalScale;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
    }

    public void Break()
    {
        StopAllCoroutines();
        StartCoroutine(DisableWall());
    }

    private IEnumerator DisableWall()
    {
        // 动画缩小
        yield return Tween.Scale(transform, originalScale, originalScale * 0.1f, 0.15f, Ease.InBack).ToYieldInstruction();

        col.enabled = false;
        sr.enabled = false;

        yield return new WaitForSeconds(disableTime);

        // 先恢复显示
        sr.enabled = true;
        col.enabled = true;

        // 动画放大
        yield return Tween.Scale(transform, originalScale * 0.1f, originalScale, 0.15f, Ease.OutBack).ToYieldInstruction();
    }
}