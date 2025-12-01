using UnityEngine;

[CreateAssetMenu(fileName = "PlayerState_Hurt", menuName = "State Machine/Player States/Hurt")]
public class PlayerState_Hurt : PlayerState
{
    [Header("死亡演出设置")]
    [SerializeField] private float jumpForce = 15f; // 向上跳的力量
    [SerializeField] private float duration = 2.5f; // 掉落过程持续时间

    private float timer;

    public override void Enter()
    {
        base.Enter();
        timer = 0f;

        // 1. 播放死亡音效
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Instance.playerDeath);
        }

        // 2. 禁用碰撞体 (关键：这样才能穿过地面掉下去)
        player.SetColliderEnabled(false);

        // 3. 施加向上的力 (马里奥式跳跃)
        player.SetVelocity(Vector2.zero); // 先清空当前速度
        player.SetVelocityY(jumpForce);

        animator.Play("Player_Hurt");
    }

    public override void LogicUpdate()
    {
        timer += Time.deltaTime;

        // 5. 时间到，执行重生
        if (timer >= duration)
        {
            player.Respawn();
        }
    }

    public override void Exit()
    {
        base.Exit();

        // 6. 退出状态时，一定要把碰撞体重新打开！否则重生后会掉出地图
        player.SetColliderEnabled(true);
    }
}