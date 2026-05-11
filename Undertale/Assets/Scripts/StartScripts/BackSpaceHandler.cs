using UnityEngine;

public class BackspaceHandler : MonoBehaviour
{
    public Name_Input nameInput;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            Debug.Log("BACKSPACE DETECTADO");
            nameInput.DeleteLetter();
        }
    }
}