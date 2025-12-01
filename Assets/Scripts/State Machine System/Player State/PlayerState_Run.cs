using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "PlayerState_Run", menuName = "State Machine/Player States/Run")]
public class PlayerState_Run : PlayerState
{
    [SerializeField] float runSpeed = 5f;

    public override void Enter()
    {
        animator.Play("Player_Run");
    }

    public override void LogicUpdate()
    {
        if (!(Keyboard.current.aKey.isPressed || Keyboard.current.dKey.isPressed))
        {
            stateMachine.SwitchState(typeof(PlayerState_Idle));
        }
    }

    public override void PhysicsUpdate()
    {
        player.Move(runSpeed);
    }

}
