using UnityEngine;
using System.Collections.Generic;

public class PlayerStateMachine : StateMachine
{
    [SerializeField] PlayerState[] states;
    Animator animator;
    public Player player;
    PlayerInput input;

    public bool IsGrounded { get; private set; }

    public enum AttackType { Primary, Secondary }
    public AttackType CurrentAttackType { get; private set; }

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        input = GetComponent<PlayerInput>();

        player = GetComponent<Player>();

        stateTable = new Dictionary<System.Type, IState>(states.Length);

        foreach (var state in states)
        {
            state.Initialize(animator, player, input, this);
            stateTable[state.GetType()] = state;
        }
    }

    void Start()
    {
        SwitchOn(stateTable[typeof(PlayerState_Grounded)]);
    }

    void Update()
    {
        IsGrounded = currentState is PlayerState_Grounded;
        currentState?.LogicUpdate();
    }

    public void SetAttackType(AttackType type)
    {
        CurrentAttackType = type;
    }
}