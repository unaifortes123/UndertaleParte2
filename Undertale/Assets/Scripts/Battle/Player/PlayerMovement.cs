using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    float xMovement;
    float yMovement;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        xMovement = Input.GetAxisRaw("Horizontal");
        yMovement = Input.GetAxisRaw("Vertical");

        rb.velocity = new Vector3(xMovement * speed, yMovement * speed);

    }

    void OnDisable()
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }
}
