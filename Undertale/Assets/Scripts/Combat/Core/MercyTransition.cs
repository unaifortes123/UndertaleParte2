using UnityEngine;

public class MercyTransition : MonoBehaviour
{
    [Header("Referencias de UI")]
    public GameObject Dialogue;
    public GameObject battleBox;

    // se llama al pulsar el boton de mercy: oculta el cuadro de dialogo y muestra el battleBox
    public void OnMercyClick()
    {
        if (Dialogue != null && battleBox != null)
        {
            Dialogue.SetActive(false);
            battleBox.SetActive(true);
        }
    }
}
