using UnityEngine;
using System;

public class CursorManager : MonoBehaviour
{
    [Header("光标贴图")]
    [SerializeField] private Texture2D defaultCursor; // 默认光标
    [SerializeField] private Texture2D chargeReadyCursor; // 蓄力完成时的光标

    private void Start()
    {
        SetCursor(defaultCursor);
    }

    private void OnEnable()
    {
        WeaponStateMachine.OnStateChanged += HandleWeaponStateChange;
    }

    private void OnDisable()
    {
        WeaponStateMachine.OnStateChanged -= HandleWeaponStateChange;
    }

    private void HandleWeaponStateChange(Type newState)
    {
        if (newState == typeof(WeaponState_ChargeReady))
        {
            SetCursor(chargeReadyCursor);
        }
        else
        {
            SetCursor(defaultCursor);
        }
    }

    private void SetCursor(Texture2D cursorTexture)
    {
        if (cursorTexture == null) return;

        Vector2 hotspot = new Vector2(cursorTexture.width / 2f, cursorTexture.height / 2f);
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }
}