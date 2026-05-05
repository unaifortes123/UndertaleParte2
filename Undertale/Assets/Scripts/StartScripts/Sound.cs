using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sound : MonoBehaviour
{
    [SerializeField]Slider VolumeSlider;
    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("Volume"))
        {
            PlayerPrefs.SetFloat("Volume", 1);
            Load();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeVolume()
    {
        AudioListener.volume = VolumeSlider.value;
        Save();
    }
    private void Load()
    {
        VolumeSlider.value = PlayerPrefs.GetFloat("Volume");
    }
    private void Save()
    {
        PlayerPrefs.SetFloat("Volume",VolumeSlider.value);
    }
}
