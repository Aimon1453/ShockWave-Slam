using UnityEngine;

public class DustParticle : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float minSpeed = 0.2f;
    [SerializeField] private float maxSpeed = 0.8f;

    [Header("生命周期")]
    [SerializeField] private float lifeTime = 5f; // 如果动画不循环，这个时间设为动画长度

    private Vector2 moveDirection;
    private float moveSpeed;

    private void Start()
    {
        // 1. 随机一个移动方向
        moveDirection = Random.insideUnitCircle.normalized;

        // 2. 随机一个速度
        moveSpeed = Random.Range(minSpeed, maxSpeed);

        // 3. 定时销毁（或者你可以用动画事件销毁）
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // 缓慢移动
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }
}