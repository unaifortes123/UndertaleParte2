using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    public PlayableDirector timeline;


    private void OnTriggerEnter2D(Collider2D collision)
    {

        timeline.Play();
    }
}
