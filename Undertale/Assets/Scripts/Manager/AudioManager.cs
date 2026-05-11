using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public List<AudioClip> clipList = new List<AudioClip>();
    public AudioSource audioSource;
    [HideInInspector]
    public static AudioManager instance;
    // Esta funcion guarda este audio manager para usarlo desde otros scripts.
    void Awake()
    {
        instance = this.GetComponent<AudioManager>();
    }



    // Esta funcion pone el sonido de recibir dano.
    public void takingDamage()
    {
        audioSource.clip = clipList[0];
        audioSource.Play();
    }

    // Esta funcion pone el sonido de moverte por el menu.
    public void Hovering()
    {
        audioSource.clip = clipList[1];
        audioSource.Play();
    }

    // Esta funcion pone el sonido de seleccionar una opcion.
    public void Selecting()
    {
        audioSource.clip = clipList[2];
        audioSource.Play();
    }

    // Esta funcion pone el sonido de atacar.
    public void Slashing()
    {
        audioSource.clip = clipList[3];
        audioSource.Play();
    }
}
