using UnityEngine;

public class Pellet : MonoBehaviour, IFightObject
{
    public Transform playerTransform;
    public PelletType type;
    private float time;

    // Pone la rotacion inicial segun el tipo, los huesos rectos hacia un lado y los del otro tipo girados 180.
    public void Spawn()
    {
        if (type == PelletType.BoneLeft)
        {
            transform.rotation = Quaternion.identity;
        }

        if (type == PelletType.BoneTopLeft)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        }

        if (type == PelletType.BoneSpinRight)
        {
            transform.rotation = Quaternion.identity;
        }

        if (type == PelletType.BoneSpinLeft)
        {
            transform.rotation = Quaternion.identity;
        }
    }

    // Cada frame mira el tipo de bala y le aplica el movimiento que toca.
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

        if (type == PelletType.BoneDown)
        {
            BoneDown();
        }

        if (type == PelletType.BoneLeft || type == PelletType.BoneTopLeft)
        {
            HandleSideRainLeft();
        }

        if (type == PelletType.BoneSpinRight)
        {
            HandleBoneSpinRight();
        }

        if (type == PelletType.BoneSpinLeft)
        {
            HandleBoneSpinLeft();
        }

        if (type == PelletType.BoneUp)
        {
            BoneUp();
        }
    }

    // Persigue al player a velocidad constante de 1 ud/seg, no le deja descansar.
    void HandleFollowDirect()
    {
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, Time.deltaTime);
    }

    // Espera 1 segundo y luego salta hacia arriba apuntando a la X del player.
    void JumpDirect()
    {
        if (time >= 1)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(playerTransform.position.x, transform.position.y * -2), Time.deltaTime * 2);
        }
    }

    // Cae recto el primer segundo y luego empieza a perseguir al player.
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

    // Cae en diagonal hacia el centro de la caja, girando, segun el lado en el que aparezca.
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

    // Cruza la pantalla recto hacia la izquierda a 3.2 ud/seg sin rotar.
    void HandleSideRainLeft()
    {
        Vector2 movement;
        float speed;

        speed = 3.2f;

        movement = new Vector2(-speed, 0f);
        transform.position += (Vector3)movement * Time.deltaTime;
    }

    // Cae recto a 1.8 ud/seg, sin rotar ni perseguir.
    void BoneDown()
    {
        transform.position += Vector3.down * Time.deltaTime * 1.8f;
    }

    // Atraviesa la caja hacia la derecha rapido mientras gira.
    void HandleBoneSpinRight()
    {
        Vector2 movement;
        float speed;
        float rotationSpeed;

        speed = 2.6f;
        rotationSpeed = 280f;

        movement = new Vector2(speed, 0f);
        transform.position += (Vector3)movement * Time.deltaTime;
        transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);
    }

    // Igual que el de la derecha pero hacia el otro lado.
    void HandleBoneSpinLeft()
    {
        Vector2 movement;
        float speed;
        float rotationSpeed;

        speed = 2.6f;
        rotationSpeed = 280f;

        movement = new Vector2(-speed, 0f);
        transform.position += (Vector3)movement * Time.deltaTime;
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    // Sube recto a 1.8 ud/seg, version contraria a BoneDown.
    void BoneUp()
    {
        transform.position += Vector3.up * Time.deltaTime * 1.8f;
    }

    // Destruye el objeto de la bala cuando termina el ataque.
    public void Remove()
    {
        Destroy(gameObject);
    }
}
