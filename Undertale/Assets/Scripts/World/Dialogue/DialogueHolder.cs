using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueHolder : MonoBehaviour
{
    [HideInInspector]
    public static DialogueHolder instance;
    public string[] dialogue;
    // Esta funcion se ejecuta al crear el objeto y prepara sus referencias iniciales.
    void Awake()
    {
        instance = this;
    }

}
