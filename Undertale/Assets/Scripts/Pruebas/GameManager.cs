using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameManager : MonoBehaviour
{
    public GameObject DialogBox;
    public TextMeshProUGUI dialogText;
    // Start is called before the first frame update
  
    public void ShowText(string text)
    {
        Debug.Log("ñam");

        DialogBox.SetActive(true);
        dialogText.text = text;
    }
    public void HideText()
    {
        DialogBox.SetActive(false);
        dialogText.text = ""; 
    }
}
