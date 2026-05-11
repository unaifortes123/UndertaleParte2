using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// componente que va en cada opcion del menu ACT, junta el GameObject con sus ActVars
public class ActingButtons : MonoBehaviour
{
    [HideInInspector]
    public GameObject instance;
    [HideInInspector]
    public bool selected;
    public Transform soulPosition; // donde se pone el alma al marcar esta opcion
    [HideInInspector]
    public ActVars actVars;

    void Awake()
    {
        instance = this.gameObject;
        actVars = this.GetComponent<ActVars>();
    }
}
