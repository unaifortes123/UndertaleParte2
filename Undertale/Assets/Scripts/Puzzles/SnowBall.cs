using UnityEngine;

public class SnowBall : MonoBehaviour
{
    public float velocidad = 4f;

    private Rigidbody2D rb;

    private bool moviendose = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // jugador empuja
        if (collision.gameObject.CompareTag("Player") && !moviendose)
        {
            Vector2 direccion =
                (transform.position - collision.transform.position).normalized;

            direccion = new Vector2(
                Mathf.Round(direccion.x),
                Mathf.Round(direccion.y)
            );

            rb.velocity = direccion * velocidad;

            moviendose = true;
        }

        // parar contra paredes
        if (collision.gameObject.CompareTag("Wall"))
        {
            rb.velocity = Vector2.zero;
            moviendose = false;
        }
    }
}