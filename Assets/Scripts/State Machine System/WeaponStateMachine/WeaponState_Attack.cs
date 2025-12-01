using UnityEngine;

[CreateAssetMenu(fileName = "WeaponState_Attack", menuName = "State Machine/Weapon States/Attack")]
public class WeaponState_Attack : WeaponState
{
    public override void Enter()
    {
        TimeManager.Instance.StopGlobalBulletTime(stateMachine.weapon);
        animator.Play("HammerAttack"); // 播放武器攻击动画
    }

    public override void Exit()
    {
    }

    private void OnAttackStarted()
    {
    }
}