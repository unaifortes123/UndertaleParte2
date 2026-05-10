using System.Collections;
using UnityEngine;

public abstract class Attacks : ScriptableObject
{
    // Cada enemigo define aqui que ataque hace en su turno.
    public abstract IEnumerator GetAttack();

    // Esta funcion crea un proyectil de combate en la posicion indicada.
    protected void SpawnPellet(Vector2 position, PelletType type, int pelletType)
    {
        AttackManager.instance.SpawnPellet(position, type, pelletType);
    }

    // Esta funcion crea una espera sencilla para las corrutinas de ataque.
    protected WaitForSeconds Wait(float seconds)
    {
        return new WaitForSeconds(seconds);
    }
}
