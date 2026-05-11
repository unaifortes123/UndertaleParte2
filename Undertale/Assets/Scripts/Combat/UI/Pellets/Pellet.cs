using UnityEngine;

public class Pellet : MonoBehaviour, IFightObject
{
    public Transform playerTransform; // referencia al player para perseguirlo
    public PelletType type; // tipo de bala/hueso, decide como se mueve
    private float time; // cuanto tiempo lleva viva la bala (algunos ataques esperan antes de empezar)

    // rotacion inicial segun el tipo. Algunos huesos vienen del techo asi que les da la vuelta
    public void Spawn()
    {
        if (type == PelletType.BoneLeft)
        {
            transform.rotation = Quaternion.identity;
        }

        if (type == PelletType.BoneTopLeft)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 180f); // boca abajo
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

    // cada frame mira el tipo y le aplica el movimiento que toque
    public void Tick()
    {
        // pilla la referencia al player cada frame por si cambia de escena o algo
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

    // persigue al player a velocidad constante (1 ud/seg), no le deja parar quieto
    void HandleFollowDirect()
    {
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, Time.deltaTime);
    }

    // espera 1 segundo y luego pega un salto hacia la X del player
    void JumpDirect()
    {
        if (time >= 1)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(playerTransform.position.x, transform.position.y * -2), Time.deltaTime * 2);
        }
    }

    // el primer segundo cae recta, despues empieza a perseguir
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

    // cae en diagonal hacia el centro, girando. Si aparece en la derecha va a la izquierda y al reves
    void HandleSideRain()
    {
        Vector3 movement;
        float sideDirection;

        sideDirection = 1;

        // si aparecio a la derecha invierte la direccion para que vaya hacia el centro
        if (transform.position.x > 0)
        {
            sideDirection = -1;
        }

        movement = new Vector3(sideDirection * 0.35f, -1.35f, 0);
        transform.position += movement * Time.deltaTime;
        transform.Rotate(0, 0, 160 * Time.deltaTime); // gira para que se vea estiloso
    }

    // cruza la pantalla recto hacia la izquierda, sin rotar (huesos de Papyrus normales)
    void HandleSideRainLeft()
    {
        Vector2 movement;
        float speed;

        speed = 3.2f;

        movement = new Vector2(-speed, 0f);
        transform.position += (Vector3)movement * Time.deltaTime;
    }

    // hueso cayendo recto, sin rotar ni perseguir
    void BoneDown()
    {
        transform.position += Vector3.down * Time.deltaTime * 1.8f;
    }

    // hueso que cruza hacia la derecha rapido girando
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

    // mismo movimiento que el spin right pero al otro lado
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

    // hueso subiendo recto (al reves del BoneDown)
    void BoneUp()
    {
        transform.position += Vector3.up * Time.deltaTime * 1.8f;
    }

    public void Remove()
    {
        Destroy(gameObject);
    }
}
