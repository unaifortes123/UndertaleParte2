using UnityEngine;

public class HoleGoal : MonoBehaviour
{
    public GameObject flag;
    public Transform holeCenter;

    private void Start()
    {
        flag.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("snowball"))
        {
            StartCoroutine(ConsumeBall(other.gameObject));
        }
    }

    System.Collections.IEnumerator ConsumeBall(GameObject ball)
    {
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        rb.simulated = false;

        Vector3 start = ball.transform.position;
        Vector3 end = holeCenter.position;

        float t = 0;
        float duration = 0.25f;

        while (t < 1)
        {
            t += Time.deltaTime / duration;
            ball.transform.position = Vector3.Lerp(start, end, t);
            ball.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
            yield return null;
        }

        Destroy(ball);

        StartCoroutine(ShowFlag());
    }

    System.Collections.IEnumerator ShowFlag()
    {
        flag.SetActive(true);
        Vector3 start = flag.transform.localScale;
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
