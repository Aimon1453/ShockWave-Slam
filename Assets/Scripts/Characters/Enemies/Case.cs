using UnityEngine;

public class Case : MonoBehaviour
{
    private Rigidbody2D rb;

    public void Initialize(Vector2 force, float torque, float gravityScale)
    {
        if (!TryGetComponent<Rigidbody2D>(out rb))
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        rb.gravityScale = gravityScale;
        rb.AddForce(force, ForceMode2D.Impulse);
        rb.AddTorque(torque, ForceMode2D.Impulse);

        Destroy(gameObject, 3f);
    }
}