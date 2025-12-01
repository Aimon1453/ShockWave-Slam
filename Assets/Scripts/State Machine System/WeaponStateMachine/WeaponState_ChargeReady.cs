using UnityEngine;

[CreateAssetMenu(fileName = "WeaponState_ChargeReady", menuName = "State Machine/Weapon States/ChargeReady")]
public class WeaponState_ChargeReady : WeaponState
{
    public override void Enter()
    {
        animator.Play("HammerChargeReady"); // 播放蓄力完成的动画
        input.AttackCanceled += OnAttackCanceled;// 订阅“蓄力后松手”事件
        input.SecondaryAttackCanceled += OnAttackCanceled;
    }

    public override void Exit()
    {
        input.AttackCanceled -= OnAttackCanceled;// 离开时取消订阅
        input.SecondaryAttackCanceled -= OnAttackCanceled;
        weapon.ClearEnemyTargetPoints();// 清除目标点显示
        weapon.ClearPlayerTargetPoints();// 清除玩家目标点显示
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        weapon.DetectTargetEnemy();
        weapon.DrawKnockbackDirection();
        weapon.DrawPlayerRecoilTrajectory();

    }

    private void OnAttackCanceled()
    {
        stateMachine.SwitchState(typeof(WeaponState_Attack));// 这个状态松手，蓄力已完成，可以攻击
    }
}