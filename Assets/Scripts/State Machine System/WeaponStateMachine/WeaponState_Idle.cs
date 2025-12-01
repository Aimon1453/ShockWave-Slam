using UnityEngine;

[CreateAssetMenu(fileName = "WeaponState_Idle", menuName = "State Machine/Weapon States/Idle")]
public class WeaponState_Idle : WeaponState
{
    public override void Enter()
    {
        TimeManager.Instance.StopGlobalBulletTime(stateMachine.weapon);
        // 订阅“开始攻击”事件
        input.AttackStarted += OnAttackStarted;
        input.SecondaryAttackStarted += OnSecondaryAttack;

        animator.Play("HammerIdle"); // 播放武器待机动画
    }

    public override void Exit()
    {
        // 离开状态时，取消订阅，避免在其他状态下也响应这个事件
        input.AttackStarted -= OnAttackStarted;
        input.SecondaryAttackStarted -= OnSecondaryAttack;
    }

    private void OnAttackStarted()
    {
        // 告诉 PlayerStateMachine 这是主要攻击（左键）
        stateMachine.PlayerStateMachine.SetAttackType(PlayerStateMachine.AttackType.Primary);
        // 收到开始攻击的信号后，切换到蓄力状态
        stateMachine.SwitchState(typeof(WeaponState_Charge));
    }

    private void OnSecondaryAttack()
    {
        // 告诉 PlayerStateMachine 这是次要攻击（右键）
        stateMachine.PlayerStateMachine.SetAttackType(PlayerStateMachine.AttackType.Secondary);
        // 同样切换到蓄力状态
        stateMachine.SwitchState(typeof(WeaponState_Charge));
    }
}