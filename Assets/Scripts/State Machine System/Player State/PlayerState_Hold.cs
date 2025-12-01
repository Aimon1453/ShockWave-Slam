using UnityEngine;

[CreateAssetMenu(fileName = "PlayerState_Hold", menuName = "State Machine/Player States/Hold")]
public class PlayerState_Hold : PlayerState
{
    [SerializeField] private float holdDuration = 3f; // 持续时间

    private float timer;

    public override void Enter()
    {
        base.Enter();
        timer = 0f;
        player.SetVelocity(Vector2.zero);
        animator.Play("Player_Hold"); // 播放获得武器动画
        AudioManager.Instance.PlaySfx(AudioManager.Instance.weaponPickup); // 播放捡起武器音效
    }

    public override void LogicUpdate()
    {
        timer += Time.deltaTime;
        if (timer >= holdDuration)
        {
            stateMachine.SwitchState(typeof(PlayerState_Grounded));
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.weaponObject.SetActive(true); // 激活子物体，此时 Weapon 的 Awake 会执行
        player.HasWeapon = true;
    }
}