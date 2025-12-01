using UnityEngine;

public class PlayerState : ScriptableObject, IState
{
    protected Animator animator;

    protected Player player;
    protected PlayerInput input;
    protected PlayerStateMachine stateMachine;

    public void Initialize(Animator animator, Player player, PlayerInput playerInput, PlayerStateMachine stateMachine)
    {
        this.animator = animator;
        this.player = player;
        this.input = playerInput;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter()
    {
        // Code to execute when entering the state
    }

    public virtual void Exit()
    {
        // Code to execute when exiting the state
    }

    public virtual void LogicUpdate()
    {
        // Code for logic updates
    }

    public virtual void PhysicsUpdate()
    {
        // Code for physics updates
    }
}