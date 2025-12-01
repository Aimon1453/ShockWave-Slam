using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerInput : MonoBehaviour
{
    // 定义四个事件，让其他脚本（比如你的武器状态机）可以监听
    public event Action AttackStarted; // 按下左键时触发
    public event Action AttackCanceled;

    public event Action SecondaryAttackStarted; // 新增：按下右键时触发
    public event Action SecondaryAttackCanceled; // 新增：松开右键时触发

    PlayerInputActions playerInputActions;
    InputAction attackAction;
    InputAction secondaryAttackAction;

    Vector2 axes => playerInputActions.GamePlay.Axes.ReadValue<Vector2>();
    public float AxisX => axes.x;
    public bool Move => AxisX != 0f;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        attackAction = playerInputActions.GamePlay.Attack;
        secondaryAttackAction = playerInputActions.GamePlay.SecondaryAttack;
    }

    private void OnEnable()
    {
        // 1. 启用输入图
        playerInputActions.GamePlay.Enable();

        // 2. 订阅攻击事件
        attackAction.started += HandleAttackStarted;
        attackAction.canceled += HandleAttackCanceled;

        // 3. 订阅二次攻击事件
        secondaryAttackAction.started += HandleSecondaryAttackStarted;
        secondaryAttackAction.canceled += HandleSecondaryAttackCanceled;
    }

    // 当脚本被禁用时，清理所有东西
    private void OnDisable()
    {
        playerInputActions.GamePlay.Disable();
        attackAction.started -= HandleAttackStarted;
        attackAction.canceled -= HandleAttackCanceled;

        secondaryAttackAction.started -= HandleSecondaryAttackStarted;
        secondaryAttackAction.canceled -= HandleSecondaryAttackCanceled;
    }

    void OnDestroy() => playerInputActions.Dispose();

    private void HandleAttackStarted(InputAction.CallbackContext ctx) => AttackStarted?.Invoke();
    private void HandleAttackCanceled(InputAction.CallbackContext ctx) => AttackCanceled?.Invoke();

    private void HandleSecondaryAttackStarted(InputAction.CallbackContext ctx) => SecondaryAttackStarted?.Invoke();
    private void HandleSecondaryAttackCanceled(InputAction.CallbackContext ctx) => SecondaryAttackCanceled?.Invoke();
}
