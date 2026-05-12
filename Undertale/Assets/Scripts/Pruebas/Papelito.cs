using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Papelito : MonoBehaviour
{
    [SerializeField] private GameObject papel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
     void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            papel.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            papel.SetActive(false);
        }
    }
}
