using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine 
{
    [TextArea(3, 5)]
    public string text;

    public AudioClip clip;

    public Sprite portrait;

	public string animationName;
}
