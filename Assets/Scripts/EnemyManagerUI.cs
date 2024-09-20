using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyManagerUI : MonoBehaviour
{
    public TextMeshProUGUI killCountNumberText;

    private EnemyManager enemyManager;

    private void Start()
    {
        enemyManager = GetComponent<EnemyManager>();

        if (PlayerPrefs.HasKey("KillCount"))
        {
            killCountNumberText.text = PlayerPrefs.GetInt("KillCount").ToString("D4");
        }
        else
        {
            killCountNumberText.text = 0.ToString("D4");
        }

        enemyManager.OnEnemyCountChanged += EnemyManager_OnEnemyCountChanged;
    }
    private void OnDisable()
    {
        enemyManager.OnEnemyCountChanged -= EnemyManager_OnEnemyCountChanged;
    }

    private void EnemyManager_OnEnemyCountChanged(int obj)
    {
        string text = obj.ToString("D4");
        killCountNumberText.text = text;
    }
}
