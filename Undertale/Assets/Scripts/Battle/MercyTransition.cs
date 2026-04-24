using UnityEngine;

public class MercyTransition : MonoBehaviour
{
    [Header("Referencias de UI")]
    public GameObject Dialogue;
    public GameObject battleBox;

    // Este método se llamará al pulsar el botón
    public void OnMercyClick()
    {
        if (Dialogue != null && battleBox != null)
        {
            // 1. Desactivamos el diálogo actual
            Dialogue.SetActive(false);

            // 2. Activamos el nuevo cuadro/interfaz
            battleBox.SetActive(true);

            Debug.Log("Acción MERCY ejecutada.");
        }
    }
}