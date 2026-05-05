using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TxtHealthEnemic : MonoBehaviour
{

    public EnemyVars statsEnemy;
    public TextMeshPro miTextoTMP;

    // Esta funcion pone la vida maxima del enemigo al empezar.
    void Start()
    {
        miTextoTMP.text = statsEnemy.maxHP.ToString();

    }

    // Esta funcion actualiza el texto con la vida actual del enemigo.
    void Update()
    {
        miTextoTMP.text = statsEnemy.curHP.ToString();
    }
}
