using UnityEngine;

[CreateAssetMenu(fileName = "PlayerState_Victory", menuName = "State Machine/Player States/Victory")]
public class PlayerState_Victory : PlayerState
{
    [SerializeField] private float poseDuration = 2f;

    private float timer;

    public override void Enter()
    {
        base.Enter();
        timer = 0f;

        // 停止玩家移动
        player.SetVelocity(Vector2.zero);
        player.weaponObject.SetActive(false); // 隐藏武器

        // 播放胜利/收集完成动画
        animator.Play("Player_Victory");

        AudioManager.Instance.PlaySfx(AudioManager.Instance.weaponPickup); // 播放胜利音效
    }

    public override void LogicUpdate()
    {
        timer += Time.deltaTime;

        if (timer >= poseDuration)
        {
            stateMachine.SwitchState(typeof(PlayerState_Grounded));
            player.weaponObject.SetActive(true); // 显示武器
        }
    }


}