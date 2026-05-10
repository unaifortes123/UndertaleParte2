using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLogic : MonoBehaviour
{
    public EnemyVars enemy;
    PlayerVars player;
    public List<Sprite> projectileSprite;
    int spriteShown;
    float time;
    public float shownTime;
    // Esta funcion se ejecuta al empezar la escena y deja preparado el componente.
    void Start()
    {
        enemy = FindObjectOfType<EnemyVars>().GetComponent<EnemyVars>();
    }
    // Esta funcion se ejecuta al crear el objeto y prepara sus referencias iniciales.
    void Awake()
    {
        GameObject pelleteParent = GameObject.FindGameObjectWithTag("PelleteHolder");
        this.transform.parent = pelleteParent.transform;
    }

    // Esta funcion detecta una colision con otro objeto.
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player = PlayerVars.instance;

            if (player != null)
            {
                player.TakeDamage(enemy.attackValue - player.defValue);
            }
        }
    }
}
