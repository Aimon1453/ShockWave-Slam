using UnityEngine;

[CreateAssetMenu(fileName = "PlayerState_InAir", menuName = "State Machine/Player States/InAir")]
public class PlayerState_InAir : PlayerState
{
    // 在这里可以设置空中的移动速度、跳跃高度等
    [SerializeField] private float airMoveSpeed = 3.5f;
    [Header("跳跃优化")]
    [SerializeField] private float upwardGravityScale = 2f; // 上升阶段的重力
    [SerializeField] private float downwardGravityScale = 5f; // 下落阶段的重力

    public override void LogicUpdate()
    {
        if (player.CheckIfGrounded())
        {
            stateMachine.SwitchState(typeof(PlayerState_Grounded));
            return;
        }

        player.FlipTowardsMouse();
        player.Move(airMoveSpeed);
        HandleGravity();
        CheckAnimation();
    }

    private void HandleGravity()
    {
        if (player.rb.linearVelocity.y < 0)
        {
            player.rb.gravityScale = downwardGravityScale;
        }
        else
        {
            player.rb.gravityScale = upwardGravityScale;
        }
    }

    private void CheckAnimation()
    {
        animator.Play("Player_Spin");
    }
}