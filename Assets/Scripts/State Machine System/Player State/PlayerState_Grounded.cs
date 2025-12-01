using UnityEngine;

[CreateAssetMenu(fileName = "PlayerState_Grounded", menuName = "State Machine/Player States/Grounded")]
public class PlayerState_Grounded : PlayerState
{
    [SerializeField] private float moveSpeed = 8f;

    public override void LogicUpdate()
    {
        // 1. 如果不再接触地面，切换到空中状态
        if (!player.CheckIfGrounded())
        {
            stateMachine.SwitchState(typeof(PlayerState_InAir));
            return;
        }

        // 2. 处理地面移动
        player.Move(moveSpeed);
        player.FlipTowardsMouse();

        // 3. 根据输入播放动画
        if (input.Move)
        {
            animator.Play("Player_Run");
        }
        else
        {
            animator.Play("Player_Idle");
        }
    }
}