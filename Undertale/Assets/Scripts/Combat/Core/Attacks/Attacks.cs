using System.Collections;
using UnityEngine;

// clase base de los Attacks scriptable. Cada enemigo hereda y mete sus propios ataques
public abstract class Attacks : ScriptableObject
{
    // cada enemigo decide aqui que ataque toca en su turno
    public abstract IEnumerator GetAttack();

    // helper para spawnear una bala sin tener que escribir AttackManager.instance.SpawnPellet cada vez
    protected void SpawnPellet(Vector2 position, PelletType type, int pelletType)
    {
        AttackManager.instance.SpawnPellet(position, type, pelletType);
    }

    // helper para hacer "yield return Wait(0.5f)" mas corto que escribir new WaitForSeconds
    protected WaitForSeconds Wait(float seconds)
    {
        return new WaitForSeconds(seconds);
    }
}
