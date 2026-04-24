using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TxtHealthEnemic : MonoBehaviour
{

    public EnemyVars statsEnemy;
    public TextMeshPro miTextoTMP;

    // Start is called before the first frame update
    void Start()
    {
        miTextoTMP.text = statsEnemy.maxHP.ToString(); // Damos el texto de la vida al maximo.

    }

    // Update is called once per frame
    void Update()
    {
        miTextoTMP.text = statsEnemy.curHP.ToString(); // Se actualiza cada frame.
    }
}
