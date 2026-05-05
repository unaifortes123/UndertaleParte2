using UnityEngine;

public class Pellet : MonoBehaviour, IFightObject
{
    public Transform playerTransform;
    public PelletType type;
    private float time;

    // Esta funcion prepara la bala al aparecer.
    public void Spawn()
    {
    }

    // Esta funcion mueve la bala segun su tipo.
    public void Tick()
    {
        playerTransform = FindObjectOfType<PlayerMovement>().transform;
        time += Time.deltaTime;

        if (type == PelletType.FollowDirect)
        {
            HandleFollowDirect();
        }
        else if (type == PelletType.FallFollowDirect)
        {
            HandleFall();
        }

        if (type == PelletType.JumpDirect)
        {
            JumpDirect();
        }

        if (type == PelletType.SideRain)
        {
            HandleSideRain();
        }
    }

    // Esta funcion hace que la bala siga al player.
    void HandleFollowDirect()
    {
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, Time.deltaTime);
    }

    // Esta funcion hace que la bala salte hacia el player.
    void JumpDirect()
    {
        if (time >= 1)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(playerTransform.position.x, transform.position.y * -2), Time.deltaTime * 2);
        }
    }

    // Esta funcion hace que la bala caiga y luego persiga al player.
    void HandleFall()
    {
        if (time < 1)
        {
            transform.position += Vector3.down * Time.deltaTime;
        }
        else
        {
            HandleFollowDirect();
        }
    }

    // Esta funcion hace caer la bala desde los lados.
    void HandleSideRain()
    {
        Vector3 movement;
        float sideDirection;

        sideDirection = 1;

        if (transform.position.x > 0)
        {
            sideDirection = -1;
        }

        movement = new Vector3(sideDirection * 0.35f, -1.35f, 0);
        transform.position += movement * Time.deltaTime;
        transform.Rotate(0, 0, 160 * Time.deltaTime);
    }

    // Esta funcion borra la bala al terminar el ataque.
    public void Remove()
    {
        Destroy(gameObject);
    }

}
