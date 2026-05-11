using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine : MonoBehaviour
{
    [TextArea(3, 5)]
    public string text;

    public AudioClip clip;
}
