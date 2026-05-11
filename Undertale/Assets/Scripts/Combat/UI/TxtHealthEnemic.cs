using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// pone el numero de vida del enemigo encima de su sprite y lo va actualizando segun le pega
public class TxtHealthEnemic : MonoBehaviour
{

    public EnemyVars statsEnemy;
    public TextMeshPro miTextoTMP;

    void Start()
    {
        miTextoTMP.text = statsEnemy.maxHP.ToString();

    }

    void Update()
    {
        miTextoTMP.text = statsEnemy.curHP.ToString();
    }
}
