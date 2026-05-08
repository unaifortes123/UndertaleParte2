using UnityEngine;

public class HoleGoal : MonoBehaviour
{
    //llamamos al game Object de la bandera
    public GameObject flag;
    //Ubicación del centro del agujero
    public Transform holeCenter;

    private void Start()
    {
        //que esté desactivada al empezar
        flag.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //cuando colisione con snowball 
        if (other.CompareTag("snowball"))
        {//haremos que del circulo aparezca poco a poco la flecha mas adelante 
            StartCoroutine(ConsumeBall(other.gameObject));
        }
    }

    System.Collections.IEnumerator ConsumeBall(GameObject ball)
    {
        //cogemos el rigid body de la bola
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        //la velocidad se volvera a 0
        rb.simulated = false;

        Vector3 start = ball.transform.position;
        Vector3 end = holeCenter.position;

        float t = 0;
        float duration = 0.25f;

        while (t < 1)
        {//le sumaremos el delta time dividido x duracion 
            t += Time.deltaTime / duration;
            ball.transform.position = Vector3.Lerp(start, end, t);
            ball.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
            yield return null;
        }

        Destroy(ball);
        //destruyo la bola 

        StartCoroutine(ShowFlag());
    }

    System.Collections.IEnumerator ShowFlag()
    {
        flag.SetActive(true);
        //en el método de enseñar la bandera la pondremos en true
        Vector3 start = flag.transform.localScale;
        //aparecerá poco a poco
        flag.transform.localScale = Vector3.zero;

        float t = 0;
        float duration = 0.4f;

        while (t < 1)
        {
            t += Time.deltaTime / duration;
            flag.transform.localScale = Vector3.Lerp(Vector3.zero, start, t);
            yield return null;
        }
    }
}