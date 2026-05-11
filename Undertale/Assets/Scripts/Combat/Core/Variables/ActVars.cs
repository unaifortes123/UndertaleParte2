using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// datos que tiene cada opcion del menu ACT (frases que dice y cuanta piedad da)
public class ActVars : MonoBehaviour
{
    public List<string> actTxt; // frases que va diciendo la opcion al usarla (van rotando)
    public List<int> mercyValue; // mercy que suma cada frase
    public int mercyMax; // tope de mercy para llegar al spare
    public int curMercy; // mercy acumulado hasta ahora
}
