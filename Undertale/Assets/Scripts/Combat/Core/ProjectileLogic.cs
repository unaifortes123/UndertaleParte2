using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// componente que va en cada bala/hueso para que aplique daño al player al chocar
public class ProjectileLogic : MonoBehaviour
{
    public EnemyVars enemy; // de quien viene la bala, para coger su attackValue
    PlayerVars player;
    public List<Sprite> projectileSprite; // lista de sprites por si la bala anima entre varios
    int spriteShown;
    float time;
    public float shownTime;

    void Start()
    {
        // pilla al enemigo activo en la escena para saber cuanto daño hace
        enemy = FindObjectOfType<EnemyVars>().GetComponent<EnemyVars>();
    }

    void Awake()
    {
        // mete la bala dentro del PelleteHolder para tenerlas todas agrupadas en la jerarquia
        GameObject pelleteParent = GameObject.FindGameObjectWithTag("PelleteHolder");
        this.transform.parent = pelleteParent.transform;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // si choca con el player le aplica daño = ataque del enemigo - defensa del player
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
