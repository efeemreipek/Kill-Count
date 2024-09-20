using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KillCountManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI killCountText;


    private void Start()
    {
        if (PlayerPrefs.HasKey("KillCount"))
        {
            killCountText.text = PlayerPrefs.GetInt("KillCount").ToString("D4");
        }
        else
        {
            killCountText.text = 0.ToString("D4");
        }
    }
}
