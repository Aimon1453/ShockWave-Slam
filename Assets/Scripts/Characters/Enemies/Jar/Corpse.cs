using UnityEngine;

public class Corpse : MonoBehaviour
{
    [SerializeField] private float randomTorque = 10f;
    [SerializeField] private float randomSpeedJitter = 1f;
    private Rigidbody2D rb;

    public void Init(Vector2 direction, float speed)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        var finalSpeed = speed + Random.Range(-randomSpeedJitter, randomSpeedJitter);
        rb.linearVelocity = direction.normalized * finalSpeed;
        rb.AddTorque(Random.Range(-randomTorque, randomTorque), ForceMode2D.Impulse);
    }
}