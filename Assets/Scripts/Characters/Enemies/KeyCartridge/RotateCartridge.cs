using UnityEngine;

public class RotateCartridge : Cartridge, IRotatable
{

    protected override void Awake()
    {
        base.Awake();
        rb.gravityScale = 0f; // 关闭重力影响
    }
    public void SetRotation(float angle)
    {
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public float GetRotation()
    {
        return transform.eulerAngles.z;
    }
}