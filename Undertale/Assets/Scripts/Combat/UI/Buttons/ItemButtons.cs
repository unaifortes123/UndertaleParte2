using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// datos de cada item del menu ITEMS (nombre, cuanto cura, si ya se gasto)
public class ItemButtons : MonoBehaviour
{
    [HideInInspector]
    public GameObject instance;
    [HideInInspector]
    public bool selected;
    [HideInInspector]
    public bool used; // si ya se uso, no aparece en el menu
    public Transform soulPosition;
    public string itemName;
    public float itemHeal; // cuantos HP cura al usarlo
}
