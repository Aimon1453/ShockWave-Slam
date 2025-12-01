// 新建文件: PlayerState_Recoil.cs
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerState_Recoil", menuName = "State Machine/Player States/Recoil")]
public class PlayerState_Recoil : PlayerState
{
    [Tooltip("反作用力状态下的固定重力")]
    [SerializeField] private float recoilGravityScale = 1.5f;
    [SerializeField] private float minRecoilTime = 0.15f;
    private float timer;

    public override void Enter()
    {
        base.Enter();
        player.rb.gravityScale = recoilGravityScale;

        player.ResetVelocity();
        player.AddImpulseForce(player.RecoilForce);

        animator.Play("Player_Spin");
        timer = 0f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        player.FlipTowardsMouse();
        timer += Time.deltaTime;

        if (timer > minRecoilTime && player.CheckIfGrounded())
        {
            stateMachine.SwitchState(typeof(PlayerState_Grounded));
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.ClearRecoilForce();
    }
}