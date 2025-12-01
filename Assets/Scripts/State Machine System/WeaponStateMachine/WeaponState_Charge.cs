using UnityEngine;

[CreateAssetMenu(fileName = "WeaponState_Charge", menuName = "State Machine/Weapon States/Charge")]
public class WeaponState_Charge : WeaponState
{
    [SerializeField] private float chargeTime = 0.4f; // 在这里定义蓄力时间，和Input System解耦
    [Header("特效设置")]
    [SerializeField] private Color flashColor = Color.yellow; // 闪烁的目标颜色
    [SerializeField] private float flashSpeed = 10f; // 闪烁速度
    [Header("时间放缩尺度")]
    [SerializeField] private float timeSlowScale = 0.05f; // 时间放缩的目标比例

    private Color originalColor;
    private float timer;

    public override void Enter()
    {
        animator.Play("HammerCharge"); // 播放武器蓄力动画
        timer = 0f;
        originalColor = spriteRenderer.color;
        input.AttackCanceled += OnAttackCanceled;

        if (!stateMachine.player.CheckIfGrounded())
        {
            TimeManager.Instance.StartGlobalTimeSlowWithExemption(timeSlowScale, Mathf.Infinity, stateMachine.weapon);
        }
    }

    public override void Exit()
    {
        input.AttackCanceled -= OnAttackCanceled;
        spriteRenderer.color = originalColor;
    }

    public override void LogicUpdate()
    {
        timer += Time.unscaledDeltaTime;
        if (timer >= chargeTime)
        {
            stateMachine.SwitchState(typeof(WeaponState_ChargeReady));
            return;
        }

        if (spriteRenderer != null)
        {
            float pingPongValue = Mathf.PingPong(Time.unscaledTime * flashSpeed, 1f);
            spriteRenderer.color = Color.Lerp(originalColor, flashColor, pingPongValue);
        }
    }

    private void OnAttackCanceled()
    {
        // 在这个状态下，任何时候松手都意味着“提前松手”
        stateMachine.SwitchState(typeof(WeaponState_Idle));
    }
}