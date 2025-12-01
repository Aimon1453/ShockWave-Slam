using UnityEngine;

public abstract class WeaponState : ScriptableObject, IState
{
    protected WeaponStateMachine stateMachine;
    protected Animator animator;
    protected PlayerInput input;
    protected SpriteRenderer spriteRenderer;
    protected Weapon weapon;

    public void Initialize(WeaponStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        this.animator = stateMachine.Animator;
        this.input = stateMachine.Input;
        this.spriteRenderer = stateMachine.SpriteRenderer;
        this.weapon = stateMachine.weapon;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void LogicUpdate() { }
    public virtual void PhysicsUpdate() { }
}