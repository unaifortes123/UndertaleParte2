using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemButtons : MonoBehaviour
{
    // Este script guarda los datos de cada objeto del menu ITEMS.
    [HideInInspector]
    public GameObject instance;
    [HideInInspector]
    public bool selected;
    public Transform soulPosition;
    public string itemName;
    public float itemHeal;
}
