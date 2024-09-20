using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    public float mouseSensitivity;

    private void Awake()
    {
        Instance = this;

        AudioListener.volume = PlayerPrefs.GetFloat("Volume");
        mouseSensitivity = PlayerPrefs.GetFloat("Sensitivity");

    }
}
