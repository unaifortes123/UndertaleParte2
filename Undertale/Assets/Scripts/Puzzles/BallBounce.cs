using UnityEngine;

public class BallBounce : MonoBehaviour
{
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 normal = collision.contacts[0].normal;
        rb.velocity = Vector2.Reflect(rb.velocity, normal);
    }
}
