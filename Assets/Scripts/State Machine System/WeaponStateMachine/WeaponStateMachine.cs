using UnityEngine;
using System.Collections.Generic;
using System;

public class WeaponStateMachine : StateMachine
{
    [SerializeField] WeaponState[] states;
    public Animator Animator { get; private set; }
    public PlayerInput Input { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }
    public PlayerStateMachine PlayerStateMachine { get; private set; }
    public Weapon weapon { get; private set; }
    public Player player { get; private set; }

    public static event Action<Type> OnStateChanged;

    void Awake()
    {
        Animator = GetComponentInChildren<Animator>();
        Input = GetComponentInParent<PlayerInput>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        PlayerStateMachine = GetComponentInParent<PlayerStateMachine>();
        weapon = GetComponent<Weapon>();
        player = GetComponentInParent<Player>();

        stateTable = new Dictionary<System.Type, IState>(states.Length);
        foreach (var state in states)
        {
            state.Initialize(this);
            stateTable[state.GetType()] = state;
        }
    }

    void Start()
    {
        SwitchOn(stateTable[typeof(WeaponState_Idle)]);
    }

    public void OnAttackAnimationEnd()
    {
        SwitchState(typeof(WeaponState_Idle));
    }

    public override void SwitchState(Type newStateType)
    {
        base.SwitchState(newStateType);

        OnStateChanged?.Invoke(newStateType);
    }
}