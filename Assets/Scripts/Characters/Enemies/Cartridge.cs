using UnityEngine;

public class Cartridge : MultipartEnemy, IShootable
{
    [Header("子物体引用")]
    [SerializeField] private SpriteRenderer caseSpriteRenderer;

    [Header("弹壳状态图")]
    [SerializeField] private Sprite caseIdleSprite; // 弹壳在地面时的Sprite
    [SerializeField] private Sprite caseAirSprite;  // 弹壳在空中时的Sprite

    public void OnShot(Vector2 direction)
    {
        if (direction == Vector2.zero)
        {
            direction = transform.up; // 默认朝向上方
        }

        EjectCase();// 1. 抛出弹壳
        LaunchBullet(direction);// 2. 发射弹头
        Destroy(gameObject); // 3. 父物体自身“死亡”并消失
    }


    protected override void SwitchToIdleState()
    {
        caseSpriteRenderer.sprite = caseIdleSprite;
    }

    protected override void SwitchToAirState()
    {
        caseSpriteRenderer.sprite = caseAirSprite;
    }
}